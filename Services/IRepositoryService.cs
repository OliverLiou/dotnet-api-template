using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quickly_PriceQuotationApi;
using TemplateApi.Interface;
using TemplateApi.Models;

namespace TemplateApi.Services
{
    public interface IRepositoryService
    {
        /// <summary>
        /// 取得單筆資料
        /// </summary>
        Task<T?> GetDataWithIdAsync<T>(object[] id) where T : class;

        /// <summary>
        /// 新增或更新單筆資料
        /// </summary>
        Task SaveSingleDataAsync<T, T2>(T entity, string editorName) 
            where T : class
            where T2 : class;

        /// <summary>
        /// 新增或更新多筆資料
        /// </summary>
        Task SaveMutipleDataAsync<T, T2>(List<T> entitys, string editorName) 
            where T : class
            where T2 : class;

        /// <summary>
        /// 取得整個Table資料
        /// </summary>
        Task<List<T>> GetAllDataAsync<T>() where T : class;

        /// <summary>
        /// 找出範圍內的資料, 可下條件式、排序
        /// </summary>
        Task<Tuple<List<T>, int>> FindDataAsync<T>(int currentPage, int pageSize, string? querySearch,
                                                   Expression<Func<T, bool>>? predicate,
                                                   List<(string, bool)> sortColumns) where T : class;

    }

    public class RepositoryService(TemplateContext context, IConfiguration configuration, IMapper mapper) : IRepositoryService
    {
        private readonly TemplateContext _context = context;
        // private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;
        private readonly string _create = configuration.GetSection("MethodName")["Create"]!;
        private readonly string _update = configuration.GetSection("MethodName")["Update"]!;
        private readonly string _remove = configuration.GetSection("MethodName")["Remove"]!;

        public async Task<T?> GetDataWithIdAsync<T>(object[] id) where T : class
        {
            try
            {
                var result = await _context.Set<T>().FindAsync(id);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveSingleDataAsync<T, T2>(T entity, string editorName)
            where T : class
            where T2 : class
        {
            try
            {
                var _transaction = await _context.Database.BeginTransactionAsync();

                var id = GetPrimaryKeyValues(entity);
                var oldEntity = await GetDataWithIdAsync<T>(id!);
                var methodName = _create;

                if (oldEntity == null)
                    await _context.Set<T>().AddAsync(entity);    
                else
                {
                    methodName = _update;
                    _context.Entry(oldEntity).State = EntityState.Modified;
                    _context.Entry(oldEntity).CurrentValues.SetValues(entity);
                }

                await _context.SaveChangesAsync(); //如果為insert資料, 先存檔才可以取得Id

                var log = new Log() { Method = methodName, EditorName = editorName, ExcuteTime = DateTime.Now };

                var entityLog = CreateLog<T, T2>(entity, log);
                await _context.Set<T2>().AddAsync(entityLog);

                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    
        public async Task SaveMutipleDataAsync<T, T2>(List<T> entitys, string editorName) where T : class where T2 : class
        {
            try
            {
                var _transaction = await _context.Database.BeginTransactionAsync();
                
                var log = new Log() { EditorName = editorName, Method = _create, ExcuteTime = DateTime.Now };
                
                foreach(var entity in entitys)
                {
                    var id = GetPrimaryKeyValues(entity);
                    var oldEntity = await GetDataWithIdAsync<T>(id!);
                    if (oldEntity == null)
                    {
                        await _context.Set<T>().AddAsync(entity);
                        log.Method = _create;
                    }
                    else
                    {
                        _context.Entry(oldEntity).State = EntityState.Modified;
                        _context.Entry(oldEntity).CurrentValues.SetValues(entity);
                        log.Method = _update;
                    }
                
                    await _context.SaveChangesAsync();
                    
                    //建立Log
                    var entityLog = CreateLog<T, T2>(entity, log);
                    await _context.Set<T2>().AddAsync(entityLog);
                }

                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<T>> GetAllDataAsync<T>() where T : class
        {
            try
            {
                var items = await _context.Set<T>().ToListAsync();

                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Tuple<List<T>, int>> FindDataAsync<T>(int currentPage, int pageSize, string? querySearch, 
                                                                Expression<Func<T, bool>>? predicate,
                                                                List<(string, bool)>? sortColumns) where T : class
        {
            try
            {
                var items = _context.Set<T>().AsQueryable();

                //篩選條件
                if (predicate != null)
                    items = items.Where(predicate);

                if (!string.IsNullOrEmpty(querySearch))
                    items = PublicMethod.setWhereStr(querySearch, typeof(T).GetProperties(), items);

                var total = await items.CountAsync();

                //排序
                if (sortColumns != null)
                {
                    foreach(var sortColumn in sortColumns)
                    {
                        var (propertyName, isAscending) = sortColumn;
                        var parameter = Expression.Parameter(typeof(T), "x");
                        var property = Expression.Property(parameter, propertyName);
                        var lambda = Expression.Lambda(property, parameter);
                        var methodName = isAscending ? "OrderBy" : "OrderByDescending";

                        var genericMethod = typeof(Queryable).GetMethods().First(m => m.Name == methodName && m.GetParameters().Length == 2)
                                                             .MakeGenericMethod(typeof(T), property.Type);
                        items = (IQueryable<T>)genericMethod.Invoke(null, [items, lambda])!;
                    }
                }

                items = items.Skip((currentPage - 1) * pageSize).Take(pageSize);

                return Tuple.Create(await items.ToListAsync(), total);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public object?[] GetPrimaryKeyValues<T>(T entity) where T : class
        {
            try
            {
                return _context.Entry(entity).Metadata.FindPrimaryKey()!.Properties
                               .Select(p => entity.GetType().GetProperty(p.Name)?.GetValue(entity))
                               .ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T2 CreateLog<T, T2>(T source, Log log) 
            where T : class
            where T2 : class 
        {
            try
            {
                var entityLog = _mapper.Map<T2>(source);
                var entityLogPropertys = typeof(T2).GetProperties();

                var logPropertys = log.GetType().GetProperties();

                foreach(var logProperty in logPropertys)
                {
                    var name = logProperty.Name;
                    var entityLogProperty = entityLogPropertys.First(e => e.Name == name);
                    if (entityLogProperty != null && entityLogProperty.CanWrite)
                    {
                        entityLogProperty.SetValue(entityLog, logProperty.GetValue(log));
                    }   
                }

                return entityLog;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quickly_PriceQuotationApi;
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
        Task SaveDataAsync<T>(T entity) where T : class;

        /// <summary>
        /// 取得整個Table資料
        /// </summary>
        Task<List<T>> GetAllDataAsync<T>() where T : class;

        /// <summary>
        /// 找出範圍內的資料
        /// </summary>
        Task<Tuple<List<T>, int>> FindDataAsync<T>(Expression<Func<T, bool>>? predicate, int currentPage, int pageSize, string? querySearch) where T : class;
        
    }

    public class RepositoryService(TemplateContext context) : IRepositoryService
    {
        private readonly TemplateContext _context = context;

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

        public async Task SaveDataAsync<T>(T entity) where T : class
        {
            try
            {
                var _transaction = await _context.Database.BeginTransactionAsync();

                var id = GetPrimaryKeyValues(entity);
                var oldEntity = await GetDataWithIdAsync<T>(id!);
                if (oldEntity == null)
                    await _context.Set<T>().AddAsync(entity);    
                else
                {
                    _context.Entry(oldEntity).State = EntityState.Modified;
                    _context.Entry(oldEntity).CurrentValues.SetValues(entity);
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

        public async Task<Tuple<List<T>, int>> FindDataAsync<T>(Expression<Func<T, bool>>? predicate, int currentPage, int pageSize, string? querySearch) where T : class
        {
            try
            {
                var items = _context.Set<T>().AsQueryable();

                if (predicate != null)
                    items = items.Where(predicate);

                if (!string.IsNullOrEmpty(querySearch))
                    items = PublicMethod.setWhereStr(querySearch, typeof(T).GetProperties(), items);

                var total = await items.CountAsync();

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
    }
}
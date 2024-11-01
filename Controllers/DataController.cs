using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TemplateApi.Models;
using TemplateApi.Services;

namespace TemplateApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController(IRepositoryService repositoryService) : ControllerBase
    {
        private readonly IRepositoryService _repositoryService = repositoryService;

        [HttpGet("GetData/{objectId}")]
        public async Task<IActionResult> GetData(int objectId)
        {
            try
            {
                var result = await _repositoryService.GetDataWithIdAsync<Table1>([objectId]);

                return Ok(result);
            }
            catch (Exception ex)    
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("Table1Save")]
        public async Task<IActionResult> Table1Save(Table1 table1)
        {
            try
            {
                await _repositoryService.SaveSingleDataAsync<Table1, Table1Log>(table1, "");
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("Table1MutipleSave")]
        public async Task<IActionResult> Table1MutipleSave(List<Table1> table1s)
        {
            try
            {
                await _repositoryService.SaveMutipleDataAsync<Table1, Table1Log>(table1s, "");
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("RemoveTable1Data")]
        public async Task<IActionResult> RemoveTable1Data(int table1Id)
        {
            try
            {
                await _repositoryService.DeleteSigleDataAsync<Table1, Table1Log>([table1Id], "");

                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("GetTable1s")]
        public async Task<IActionResult> GetTable1s()
        {
            try
            {
                var allData = await _repositoryService.GetAllDataAsync<Table1>();
                return Ok(allData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("FindTable1/{currentPage}/{pageSize}")]
        public async Task<IActionResult> FindTable1(int currentPage, int pageSize, string? querySearch)
        {
            try
            {
                // Expression<Func<Table1, bool>> filter = t => t.Column1 == "xxxw666";
                var sortColumns = new List<(string PropertyName, bool IsAscending)> { ("Table1Id", true) };

                var tuple = await _repositoryService.FindDataAsync<Table1>(currentPage, pageSize, querySearch, null, sortColumns);

                return Ok(tuple);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
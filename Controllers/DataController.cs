using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
                await _repositoryService.SaveDataAsync(table1);
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
    }
}
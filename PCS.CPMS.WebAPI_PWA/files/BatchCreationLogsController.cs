using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;


namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchCreationLogsController : ControllerBase
    {
        private readonly tng_cpmsContext _context;

        public BatchCreationLogsController(tng_cpmsContext context)
        {
            _context = context;
        }

        // GET: api/BatchCreationLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BatchCreationLog>>> GetBatchCreationLog()
        {
            return await _context.BatchCreationLog.ToListAsync();
        }

        // GET: api/BatchCreationLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BatchCreationLog>> GetBatchCreationLog(int id)
        {
            var batchCreationLog = await _context.BatchCreationLog.FindAsync(id);

            if (batchCreationLog == null)
            {
                return NotFound();
            }

            return batchCreationLog;
        }

        // PUT: api/BatchCreationLogs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBatchCreationLog(int id, BatchCreationLog batchCreationLog)
        {
            if (id != batchCreationLog.BatchId)
            {
                return BadRequest();
            }

            _context.Entry(batchCreationLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BatchCreationLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BatchCreationLogs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<BatchCreationLog>> PostBatchCreationLog(BatchCreationLog batchCreationLog)
        {
            _context.BatchCreationLog.Add(batchCreationLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBatchCreationLog", new { id = batchCreationLog.BatchId }, batchCreationLog);
        }

        // DELETE: api/BatchCreationLogs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BatchCreationLog>> DeleteBatchCreationLog(int id)
        {
            var batchCreationLog = await _context.BatchCreationLog.FindAsync(id);
            if (batchCreationLog == null)
            {
                return NotFound();
            }

            _context.BatchCreationLog.Remove(batchCreationLog);
            await _context.SaveChangesAsync();

            return batchCreationLog;
        }

        private bool BatchCreationLogExists(int id)
        {
            return _context.BatchCreationLog.Any(e => e.BatchId == id);
        }
    }
}

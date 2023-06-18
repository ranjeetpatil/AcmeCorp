using AcmeCorp.Core.Data;
using AcmeCorp.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorp.Controllers
{
    [ApiController]
    [Route("api/contactinfo")]
    public class ContactInfoController : ControllerBase
    {
        private readonly AcmeCorpDbContext _dbContext;

        public ContactInfoController(AcmeCorpDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactInfo>>> GetContactInfo()
        {
            return await _dbContext.ContactInfos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactInfo>> GetContactInfo(int id)
        {
            var contactInfo = await _dbContext.ContactInfos.FindAsync(id);

            if (contactInfo == null)
            {
                return NotFound();
            }

            return contactInfo;
        }

        [HttpPost]
        public async Task<ActionResult<ContactInfo>> CreateContactInfo(ContactInfo contactInfo)
        {
            _dbContext.ContactInfos.Add(contactInfo);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactInfo), new { id = contactInfo.Id }, contactInfo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContactInfo(int id, ContactInfo contactInfo)
        {
            if (id != contactInfo.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(contactInfo).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!ContactInfoExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactInfo(int id)
        {
            var contactInfo = await _dbContext.ContactInfos.FindAsync(id);
            if (contactInfo == null)
            {
                return NotFound();
            }

            _dbContext.ContactInfos.Remove(contactInfo);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactInfoExists(int id)
        {
            return _dbContext.ContactInfos.Any(ci => ci.Id == id);
        }
    }
}

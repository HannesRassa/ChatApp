using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("Backend/Group")]
    [ApiController]
    public class GroupController(GroupsRepo repo) : ControllerBase
    {
        private readonly GroupsRepo repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {       
            var result = await repo.GetAllGroups();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var group = await repo.GetGroupById(id);
            if (group == null) return NotFound();
            return Ok(group);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Group newGroup)
        {
            bool result = await repo.UpdateGroup(id,newGroup);
            return result? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveGroup([FromBody] Group newGroup)
        {
            var groupExists = await repo.GroupExistsInDb(newGroup.Id);
            if (groupExists) return Conflict();
            var result = await repo.SaveGroupToDb(newGroup);
            return CreatedAtAction(nameof(SaveGroup), new { newGroup.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] int id)
        {
            bool isDeleted = await repo.DeleteGroupById(id);    
            if (!isDeleted)  return NotFound();
            return NoContent();
        }
    }
}
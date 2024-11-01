using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class GroupsRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<Group> SaveGroupToDb(Group group)
    {
        context.Add(group);
        await context.SaveChangesAsync();
        return group;
    }

    //READ
    public async Task<List<Group>> GetAllGroups() => await context.Groups.ToListAsync(); 

    public async Task<Group?> GetGroupById(int id) => await context.Groups.FindAsync(id);
    public async Task<bool> GroupExistsInDb(int id) => await context.Groups.AnyAsync(x => x.Id == id);
    
    //UPDATE
    public async Task<bool> UpdateGroup(int id, Group group)
    {
        bool isIdsMatch = id == group.Id;
        bool groupExists = await GroupExistsInDb(id);
        if (!isIdsMatch || !groupExists) return false;
        context.Update(group);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    //DELETE
    public async Task<bool> DeleteGroupById(int id)
    {
        Group? groupInDb = await GetGroupById(id);
        if (groupInDb == null) return false;
        context.Remove(groupInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}


using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DatingApp.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            // check to see if we have any users
            if(await context.Users.AnyAsync()) return;
            // read in the json generated file to seed the DB
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            // deserialize the file content to a List of AppUser
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            // iterate through the users             
            foreach(var user in users){
                // get the hmac so we can use it 
                using var hmac = new HMACSHA512();
                // lower the username
                user.UserName = user.UserName.ToLower();
                // get the password hash, hardcode the seeded user PWD's
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                // get the salt
                user.PasswordSalt = hmac.Key;
                // add the user to the context
                context.Users.Add(user);

            }
            // fire the save changes now that we have the seeded data in the context.
            await context.SaveChangesAsync();

        }
    }
}
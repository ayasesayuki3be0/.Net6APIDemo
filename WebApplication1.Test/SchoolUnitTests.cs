using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using WebApplication1.DataContext;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Net;

namespace WebApplication1.Test
{
    public class SchoolUnitTests
    {
        [Fact]
        public async Task GetRootApi()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient() ;
            var expected = "Hello ASP.NET Core WebApplication";
            //Act
            var actual = await client.GetStringAsync("/");
            //Assert
            Assert.Equal(expected, actual);
        }
        [Fact]
        public async Task GetSchoolsApi()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient();
            var expected = new List<Models.School>();

            //Act
            var actual = await client.GetFromJsonAsync<List<Models.School>>("/schools");

            //Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task PostSchoolApi()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient();
            var expected = HttpStatusCode.Created;
            //Act
            var result = await client.PostAsJsonAsync("/addschool", new Models.School
            {
                Name = "國立中興大學",
                Logo = "nchu",
                Address = "402台中市南區興大路145號",
                Tel = "04-22873181",
                Email = "dowdot@nchu.edu.tw"
            });
            var actual = result.StatusCode;

            //Assert        
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetFindSchoolApi_ID_1_is_OK()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient();
            var school = new Models.School() {
            Name="gg",
            Email="aaa@bbb.cc",
            Logo="ggtt",
            Address="yyut",
            Tel="0404040404"
            };
            var expected = 1;
            await client.PostAsJsonAsync("/addschool", school);
            //Act
            var response = await client.GetFromJsonAsync<Models.School>("/findschool/1");
            //Assert        
            Assert.Equal(expected, response.ID );
        }

        [Fact]
        public async Task GetFindSchoolApi_ID_3_is_NotFound()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient();
            var school = new Models.School()
            {
                Name = "gg",
                Email = "aaa@bbb.cc",
                Logo = "ggtt",
                Address = "yyut",
                Tel = "0404040404"
            };

            var expected = HttpStatusCode.NotFound;

            var ID = 3;
            await client.PostAsJsonAsync("/addschool", ID);
            //Act
            var response = await client.GetAsync($"/findschool/{ID}");
            //Assert        
            Assert.Equal( expected , response.StatusCode);
        }

        [Fact]
        public async Task PutEditSchoolApi_ID_1_is_NoContent()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient();
            var school = new Models.School()
            {
                Name = "ggggg",
                Email = "aaa@bbb.cc",
                Logo = "ggtt",
                Address = "yyut",
                Tel = "0404040404"
            };
            var ID = 1;
            var expected = HttpStatusCode.NoContent;
            await client.PostAsJsonAsync("/addschool", school);
            //Act
            var response = await client.PutAsJsonAsync<Models.School>($"/editschool/{ID}", school) ;
            //Assert        
            Assert.Equal(expected, response.StatusCode);
        }


        [Fact]
        public async Task PutEditSchoolApi_ID_3_is_NotFound()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient();
            var school = new Models.School()
            {
                Name = "ggggg",
                Email = "aaa@bbb.cc",
                Logo = "ggtt",
                Address = "yyut",
                Tel = "0404040404"
            };
            var ID = 3;
            var expected = HttpStatusCode.NotFound;
            await client.PostAsJsonAsync("/addschool", school);
            //Act
            var response = await client.PutAsJsonAsync<Models.School>($"/editschool/{ID}", school);
            //Assert        
            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRemoveSchoolApi_ID_1_is_NoContent()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient();
            var school = new Models.School()
            {
                Name = "ggggg",
                Email = "aaa@bbb.cc",
                Logo = "ggtt",
                Address = "yyut",
                Tel = "0404040404"
            };
            var ID = 1;
            var expected = HttpStatusCode.NoContent;
            await client.PostAsJsonAsync("/addschool", school);
            //Act
            var response = await client.DeleteAsync($"/removeschool/{ID}");
            //Assert        
            Assert.Equal(expected, response.StatusCode);
        }


        [Fact]
        public async Task DeleteRemoveSchoolApi_ID_3_is_NotFound()
        {
            //Arrange
            await using var application = new SchoolApplication();
            var client = application.CreateClient();
            var school = new Models.School()
            {
                Name = "ggggg",
                Email = "aaa@bbb.cc",
                Logo = "ggtt",
                Address = "yyut",
                Tel = "0404040404"
            };
            var ID = 3;
            var expected = HttpStatusCode.NotFound;
            await client.PostAsJsonAsync("/addschool", school);
            //Act
            var response = await client.DeleteAsync($"/removeschool/{ID}");
            //Assert        
            Assert.Equal(expected, response.StatusCode);
        }


    }

       







    class SchoolApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<MyDb>));

                services.AddDbContext<MyDb>(options => options.UseInMemoryDatabase("TestingDB", root));
            });

            return base.CreateHost(builder);
        }
    }


}
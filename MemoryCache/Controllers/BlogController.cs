using MemoryCache.Context;
using MemoryCache.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MemoryCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly BlogDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly string cacheKey = "blogCaches";
        public BlogController(BlogDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            if(_cache.TryGetValue(cacheKey, out ICollection<Blog>? cachedBlogs))
            {
                return Ok(cachedBlogs);
            }
            var blog = await _context.Blogs.ToListAsync();
            _cache.Set(cacheKey, blog, TimeSpan.FromMinutes(30));
            return Ok(blog);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(Blog blog)
        {
            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
            _cache.Set(cacheKey, blog, TimeSpan.FromMinutes(30));
            return Ok(blog);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(Blog blog, int id)
        {
            var existingBlog = await _context.Blogs.FindAsync(id);
            if (existingBlog == null) return NotFound();
            _cache.Remove(cacheKey);
            existingBlog.Title = blog.Title;
            existingBlog.Description = blog.Description;
            _cache.Set(cacheKey, existingBlog, TimeSpan.FromMinutes(30));
            _context.Blogs.Update(existingBlog);
            await _context.SaveChangesAsync();
            return Ok(existingBlog);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var existingBlog = await _context.Blogs.FindAsync(id);
            if (existingBlog == null) return NotFound();
            _context.Blogs.Remove(existingBlog);
            await _context.SaveChangesAsync();
            _cache.Remove(cacheKey);
            return NoContent();
        }
    }
}

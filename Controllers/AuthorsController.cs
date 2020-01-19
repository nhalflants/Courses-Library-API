using System.Collections;
using System.Collections.Generic;
using System;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using CourseLibrary.API.Models;
using AutoMapper;
using CourseLibrary.API.ResourceParameters;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet()]
        [HttpHead()]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
            /*[FromQuery(Name = "category")] string mainCategory,
            [FromQuery(Name = "search")] string searchQuery,*/
            [FromQuery()] AuthorsResourceParameters authorsResourceParameters)
        {
            var authors = _courseLibraryRepository.GetAuthors(authorsResourceParameters);
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors));
        }
        [HttpGet("{authorId:guid}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId) 
        {
            /*if (!_courseLibraryRepository.AuthorExists(authorId))
                return NotFound();*/
            var author = _courseLibraryRepository.GetAuthor(authorId);
            if (author == null)
                return NotFound();
            return Ok(_mapper.Map<AuthorDto>(author));
        }
        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            var authorEntity = _mapper.Map<Entities.Author>(author);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorDto = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", 
                new { authorId = authorDto.Id }, 
                authorDto);
        }
        [HttpOptions]
        public IActionResult GetAuthorsOptions() 
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
    }
}
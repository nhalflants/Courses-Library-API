using System.Collections;
using System.Collections.Generic;
using System;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using CourseLibrary.API.Models;
using AutoMapper;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Helpers;
using System.Text.Json;

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

        [HttpGet(Name = "GetAuthors")]
        [HttpHead()]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
            /*[FromQuery(Name = "category")] string mainCategory,
            [FromQuery(Name = "search")] string searchQuery,*/
            [FromQuery()] AuthorsResourceParameters authorsResourceParameters)
        {
            var authors = _courseLibraryRepository.GetAuthors(authorsResourceParameters);
            
            var previousPageLink = authors.HasPreviousPage ? 
                CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;
            var nextPageLink = authors.HasNextPage ? 
                CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;
            
            var paginationMetadata = new {
                totalCount = authors.TotalCount,
                pageSize = authors.PageSize,
                currentPage = authors.CurrentPage,
                totalPages = authors.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X_Pagination", JsonSerializer.Serialize(paginationMetadata));

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

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorEntity = _courseLibraryRepository.GetAuthor(authorId);
            if (authorEntity == null) 
                return NotFound();

            _courseLibraryRepository.DeleteAuthor(authorEntity);
            _courseLibraryRepository.Save();

            return NoContent();
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters,
            ResourceUriType type)
        {
            switch (type) {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new {
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                        new {
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
                default:
                    return Url.Link("GetAuthors",
                        new {
                            orderBy = authorsResourceParameters.OrderBy,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
            }
        }
    }
}
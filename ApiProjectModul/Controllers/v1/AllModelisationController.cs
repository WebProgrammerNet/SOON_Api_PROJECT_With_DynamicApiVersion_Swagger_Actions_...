using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProjectModul.DataBaseGenerates;
using ApiProjectModul.DataTransferObjects;
using ApiProjectModul.Exteensions;
using ApiProjectModul.Models;
using ApiProjectModul.Query;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiProjectModul.Controllers.v1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AllModelisationController : ControllerBase
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;
        private readonly IDataBaseGenerate _dbGenerate;
        public AllModelisationController(IUrlHelper urlHelper, IMapper mapper, IDataBaseGenerate dataBaseGenerate)
        {
            _urlHelper = urlHelper;
            _mapper = mapper;
            _dbGenerate = dataBaseGenerate;
        }
        [HttpGet(Name = nameof(GetAllProducts))]
        public IActionResult GetAllProducts(ApiVersion apiVersion, [FromQuery] QueryParameters queryParameters)
        {
            List<Composition> compositions = _dbGenerate.GetAll(queryParameters).ToList();

            var allcompositionCount = compositions.Count();
            var paginationMetadata = new
            {
                totaCount = allcompositionCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allcompositionCount)
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            var links = CreateLinksForCollection(queryParameters, allcompositionCount, apiVersion);
            var toReturn = compositions.Select(x => ExpandSingleFoodItem(x, apiVersion));

            return Ok(new
            {
                value = toReturn,
                links = links
            });

        }


        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleProduct))]
        public ActionResult GetSingleProduct(ApiVersion version, int id)
        {
            Composition comp = _dbGenerate.GetSingle(id);

            if (comp == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleFoodItem(comp, version));
        }


        [HttpPost(Name = nameof(AddProduct))]
        public ActionResult<CompositionDto> AddProduct(ApiVersion version, [FromBody] CreateCompositionDto compCreateDto)
        {
            if (compCreateDto == null)
            {
                return BadRequest();
            }

            Composition toAdd = _mapper.Map<Composition>(compCreateDto);
            var compo = _mapper.Map<Composition>(compCreateDto);//This file for Test

            _dbGenerate.Add(toAdd);

            if (!_dbGenerate.Save())
            {
                throw new Exception("Creating a fooditem failed on save.");
            }

            Composition createdComp = _dbGenerate.GetSingle(toAdd.Id);

            return CreatedAtRoute(nameof(GetSingleProduct), new { version = version.ToString(), id = createdComp.Id },
                _mapper.Map<CompositionDto>(createdComp));
        }
       

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveProduct))]
        public ActionResult RemoveProduct(int id)
        {
            Composition productItem = _dbGenerate.GetSingle(id);

            if (productItem == null)
            {
                return NotFound();
            }

            _dbGenerate.Delete(id);

            if (!_dbGenerate.Save())
            {
                throw new Exception("Deleting a fooditem failed on save.");
            }

            return NoContent();
        }


        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateProduct))]
        public ActionResult<Composition> UpdateProduct(int id, [FromBody] UpdateCompositionDto productupdate)
        {
            if (productupdate == null)
            {
                return BadRequest();
            }

            var existingFoodItem = _dbGenerate.GetSingle(id);

            if (existingFoodItem == null)
            {
                return NotFound();
            }

            _mapper.Map(productupdate, existingFoodItem);

            _dbGenerate.Update(id, existingFoodItem);

            if (!_dbGenerate.Save())
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            return Ok(_mapper.Map<CompositionDto>(existingFoodItem));
        }


        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateProduct))]
        public ActionResult<CompositionDto> PartiallyUpdateProduct(int id, [FromBody] JsonPatchDocument<UpdateCompositionDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            Composition existingEntity = _dbGenerate.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            UpdateCompositionDto foodUpdateDto = _mapper.Map<UpdateCompositionDto>(existingEntity);
            patchDoc.ApplyTo(foodUpdateDto);

            TryValidateModel(foodUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(foodUpdateDto, existingEntity);
            Composition updated = _dbGenerate.Update(id, existingEntity);

            if (!_dbGenerate.Save())
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            return Ok(_mapper.Map<CompositionDto>(updated));
        }

        [HttpGet("GetRandomProduct", Name = nameof(GetRandomProduct))]
        public ActionResult GetRandomProduct()
        {
            ICollection<Composition> foodItems = _dbGenerate.GetRandomMeal();

            IEnumerable<CompositionDto> dtos = foodItems
                .Select(x => _mapper.Map<CompositionDto>(x));

            var links = new List<URLDto>();

            // self 
            links.Add(new URLDto(_urlHelper.Link(nameof(GetRandomProduct), null), "self", "GET"));

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }

        private dynamic ExpandSingleFoodItem(Composition compItem, ApiVersion apiVersion)//Bura Bax?
        {
            var links = GetLinks(compItem.Id, apiVersion);
            CompositionDto item = _mapper.Map<CompositionDto>(compItem);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }
        private IEnumerable<URLDto> GetLinks(int id, ApiVersion apiVersion)
        {
            var links = new List<URLDto>();
            var getLink = _urlHelper.Link(nameof(GetSingleProduct), new { version = apiVersion.ToString(), id = id });
            links.Add(
            new URLDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemoveProduct), new { version = apiVersion.ToString(), id = id });

            links.Add(
              new URLDto(deleteLink,
              "delete_product",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddProduct), new { version = apiVersion.ToString() });

            links.Add(
              new URLDto(createLink,
              "create_product",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdateProduct), new { version = apiVersion.ToString(), id = id });

            links.Add(
               new URLDto(updateLink,
               "update_product",
               "PUT"));

            return links;
        }
        private List<URLDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            List<URLDto> links = new List<URLDto>();

            // self 
            links.Add(new URLDto(_urlHelper.Link(nameof(GetAllProducts), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new URLDto(_urlHelper.Link(nameof(GetAllProducts), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new URLDto(_urlHelper.Link(nameof(GetAllProducts), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new URLDto(_urlHelper.Link(nameof(GetAllProducts), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new URLDto(_urlHelper.Link(nameof(GetAllProducts), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddProduct), new { version = version.ToString() });//"https://localhost:44378/api/v1/foods"

            links.Add(
               new URLDto(posturl,
               "create_food",
               "POST"));

            return links;
        }
    }
}
//https://metanit.com/sharp/aspnet5/3.3.php
//https://metanit.com/sharp/entityframeworkcore/2.2.php
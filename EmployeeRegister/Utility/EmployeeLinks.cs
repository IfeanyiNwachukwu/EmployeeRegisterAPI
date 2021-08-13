using Contracts.DataShaper;
using Entities.DataTransferObjects.ReadOnly;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace EmployeeRegister.Utility
{
    public class EmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDTO> _dataShaper;

        public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDTO> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDTO> employeesDto, string
      fields, Guid companyId, HttpContext httpContext)
        {
            // shape our collection
            var shapedEmployees = ShapeData(employeesDto, fields);
            // if httpContext contains the required media type then add links to the response
            if (ShouldGenerateLinks(httpContext))
            {
                return ReturnLinkdedEmployees(employeesDto, fields, companyId, httpContext,
                shapedEmployees);
            }

            return ReturnShapedEmployees(shapedEmployees);
        }

        private List<Entity> ShapeData(IEnumerable<EmployeeDTO> employeesDto, string fields)
            =>
         _dataShaper.ShapeData(employeesDto, fields)
         .Select(e => e.Entity)
         .ToList();

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            // extract the media type from the HttpContext
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
            // If the media type ends with hateoas, method returns true
            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas",StringComparison.InvariantCultureIgnoreCase);
        }

        // Returns employees without links
        private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees) =>
            new LinkResponse { ShapedEntities = shapedEmployees };

        // Returns employees with links
        private LinkResponse ReturnLinkdedEmployees(IEnumerable<EmployeeDTO> employeesDto,
        string fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
        {
            var employeeDtoList = employeesDto.ToList();
            //Iterate through employees and create links for each employee
            for (var index = 0; index < employeeDtoList.Count(); index++)
            {
                var employeeLinks = CreateLinksForEmployee(httpContext, companyId,
                employeeDtoList[index].Id, fields);
                shapedEmployees[index].Add("Links", employeeLinks);
            }
            // here the links have been added already to each employee, just wrapping it so we can add one more link
            var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
            var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);
            return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
        }

        private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId,
            Guid id, string fields = "")
        {
            var links = new List<Link>
                    {
                    new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany",
                    values: new { companyId, id, fields }),
                    "self",
                    "GET"),
                    new Link(_linkGenerator.GetUriByAction(httpContext,
                    "DeleteEmployeeForCompany", values: new { companyId, id }),
                    "delete_employee",
                    "DELETE"),
                    new Link(_linkGenerator.GetUriByAction(httpContext,
                    "UpdateEmployeeForCompany", values: new { companyId, id }),
                    "update_employee",
                    "PUT"),
                    new Link(_linkGenerator.GetUriByAction(httpContext,
                    "PartiallyUpdateEmployeeForCompany", values: new { companyId, id }),
                    "partially_update_employee",
                    "PATCH")
                    };
                                return links;
        }

        // Adding the General Get route for all the employees
        private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext,
        LinkCollectionWrapper<Entity> employeesWrapper)
        {
            
            employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext,
            "GetEmployeesForCompany", values: new { }),
            "self",
            "GET"));
            return employeesWrapper;
        }
    }
}


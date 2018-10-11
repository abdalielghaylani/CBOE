using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using Oracle.DataAccess.Client;
using PerkinElmer.COE.Inventory.API.Code;
using PerkinElmer.COE.Inventory.API.Models;
using Swashbuckle.Swagger.Annotations;

namespace PerkinElmer.COE.Inventory.API.Controllers
{
    [ApiVersion(Consts.apiVersion)]
    public class ContainerController : InvApiController
    {
        private const string containerQuery = @"
select c.container_id id, c.container_name name,
  (to_char(c.qty_max) || ' ' || u.unit_abreviation) container_size,
  (to_char(c.qty_remaining) || ' ' || u.unit_abreviation) qty_remaining,
  l.location_name location, s.supplier_name supplier,
  nvl2(p.first_name, '', p.first_name || nvl2(p.last_name, '', ' ')) || nvl(p.last_name, '') current_user,
  c.date_created, t.container_type_name container_type, st.container_status_name container_status
from
  inv_containers c, inv_locations l, inv_compounds p, inv_suppliers s, coedb.people p,
  inv_container_types t, inv_units u, inv_container_status st
where
  c.location_id_fk = l.location_id
  and c.compound_id_fk = p.compound_id
  and c.supplier_id_fk = s.supplier_id
  and c.current_user_id_fk = p.user_id
  and c.container_type_id_fk = t.container_type_id
  and c.unit_of_meas_id_fk = u.unit_id
  and c.container_status_id_fk = st.container_status_id
  and c.container_id =  :id";

        private ContainerData BuildContainerData(OracleDataReader reader)
        {
            return new ContainerData
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                Type = (string)reader["container_type"],
                ContainerSize = (string)reader["container_size"],
                QuantityAvailable = (string)reader["qty_remaining"],
                Location = (string)reader["location"],
                Supplier = (string)reader["supplier"],
                CurrentUser = (string)reader["current_user"],
                DateCreated = (DateTime)reader["date_created"],
                Status = (string)reader["container_status"]
            };
        }

        [HttpGet]
        [Route(Consts.apiPrefix + "containers/{id}")]
        [SwaggerOperation("Containers")]
        [SwaggerResponse(200, type: typeof(ContainerData))]
        [SwaggerResponse(400, type: typeof(Exception))]
        [SwaggerResponse(401, type: typeof(Exception))]
        [SwaggerResponse(500, type: typeof(Exception))]
        public async Task<IHttpActionResult> GetContainerById(int id)
        {
            HttpResponseMessage responseMessage;
            try
            {
                // TODO: Should check authentication and authorization
                var statusCode = HttpStatusCode.OK;
                ContainerData container = null;
                using (var command = new OracleCommand(containerQuery, Connection))
                {
                    command.Parameters.Add(new OracleParameter("id", OracleDbType.Int32, id, ParameterDirection.Input));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            container = BuildContainerData(reader);
                        }
                    }
                }
                if (container == null)
                    throw new IndexOutOfRangeException(string.Format("Cannot find the container, {0}", id));
                responseMessage = Request.CreateResponse(statusCode, container);
            }
            catch (Exception ex)
            {
                responseMessage = CreateErrorResponse(ex);
            }

            return await Task.FromResult<IHttpActionResult>(ResponseMessage(responseMessage));
        }
    }
}

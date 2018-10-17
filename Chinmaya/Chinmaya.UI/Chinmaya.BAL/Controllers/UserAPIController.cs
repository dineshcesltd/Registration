﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Chinmaya.DAL;
using Chinmaya.Models;
using System.Web.Http.Description;

namespace Chinmaya.BAL.Controllers
{
    public class UserAPIController : ApiController
    {
        Users _user = new Users();

        [ResponseType(typeof(UserModel))]
        public IHttpActionResult Post(LoginViewModel entity)
        {
            return Ok(_user.GetUserInfo(entity));
        }

        public string Get(int id)
        {
            return _user.GetRoleName(id);
        }


		[Route("api/UserAPI/PostUser")]
		[HttpPost]
		public IHttpActionResult PostUser(UserModel obj)
		{
			try
			{
				_user.PostUser(obj);
				return Ok("Success");
			}
			catch (Exception)
			{
				return Ok("Something went wrong");
			}
		}

		//// GET: api/User
		//public IEnumerable<string> Get()
		//{
		//    return _user.GetUsers();
		//}

		//// GET: api/User/5
		//public string Get(int id)
		//{
		//    return _user.GetUserName(id);
		//}

		//// POST: api/User
		//public void Post([FromBody]string value)
		//{
		//}

		//// PUT: api/User/5
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		//// DELETE: api/User/5
		//public void Delete(int id)
		//{
		//}	
	}
}
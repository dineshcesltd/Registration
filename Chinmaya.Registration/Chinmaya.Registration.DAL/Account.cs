﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinmaya.DAL;
using Chinmaya.Registration.DAL;
using Chinmaya.Registration.Models;
using AutoMapper;

namespace Chinmaya.Registration.DAL
{
	public class Account
	{
		public List<KeyValueModel> GetStateName(int id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.States.Where(s => s.CountryID == id).Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

		public List<KeyValueModel> GetCityName(int id)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Cities.Where(s => s.StateID == id).Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

        public bool IsEmailExists(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                return _ctx.Users.Any(u => u.Email == email)
                        || _ctx.FamilyMembers.Any(u => u.Email == email);
            }
        }

        public bool AreAddressDetailsMatched(ContactDetails cd)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                if (_ctx.Users.Any(u => u.HomePhone == cd.HomePhone))
                {
                    return true;
                }
                if (_ctx.Users.Any(u => u.Address.ToLower() == cd.Address.ToLower()))
                {
                    if (_ctx.Users.Any(u => u.City.ToLower() == cd.City.ToLower()))
                    {
                        if (_ctx.Users.Any(u => u.StateId == cd.State))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public EmailTemplateModel GetEmailTemplateByID(int id)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<EmailTemplate, EmailTemplateModel>();
                });
                IMapper mapper = config.CreateMapper();
                EmailTemplateModel etm = new EmailTemplateModel();
                return mapper.Map(_ctx.EmailTemplates.FirstOrDefault(et => et.ID == id), etm);
            }
        }

        public string GetUserIdByEmail(string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    using (var _ctx = new ChinmayaEntities())
                    {
                        return _ctx.Users.SingleOrDefault(u => u.Email == email).Id;
                    }
                }
                return string.Empty;
            } catch(Exception)
            {
                return string.Empty;
            }
            
        }

        public List<SecurityQuestionsModel> GetSecurityQuestionsByEmail(string email)
        {
            using (var _ctx = new ChinmayaEntities())
            {
                List<SecurityQuestionsModel> qlist = new List<SecurityQuestionsModel>();
                string userId = GetUserIdByEmail(email);
                if(!string.IsNullOrEmpty(userId))
                {
                    qlist = (from usq in _ctx.UserSecurityQuestions
                             join sq in _ctx.SecurityQuestions
                             on usq.SecurityQuestionId equals sq.Id
                             where usq.UserId == userId
                             select new SecurityQuestionsModel
                             {
                                 Id = sq.Id,
                                 Name = sq.Name,
                                 Value = usq.Answer
                             }).ToList();
                }
                
                return qlist;
            }
        }

        public string GetFamilyPrimaryAccountEmail(string email)
        {
            string result = string.Empty;
            using (var _ctx = new ChinmayaEntities())
            {
                var objFamilyMembers = _ctx.FamilyMembers.FirstOrDefault(x => x.Email == email);
                if (objFamilyMembers != null)
                {
                    var objUser =_ctx.Users.FirstOrDefault(x => x.Email == objFamilyMembers.Email);
                    if (objUser != null)
                    {
                        return _ctx.Users.FirstOrDefault(x => x.Id == objFamilyMembers.UpdatedBy).Email;
                    }
                }
                else
                {
                    if (IsEmailExists(email))
                    {
                        return email;
                    }
                }
            }
            return result;
        }

        public string GetUserFullNameByEmail(string email)
        {
            if(!string.IsNullOrEmpty(email))
            {
                using (var _ctx = new ChinmayaEntities())
                {
                    var objUser = _ctx.Users.FirstOrDefault(x => x.Email == email);
                    return objUser.FirstName + " " + objUser.LastName;
                }
            }
            return string.Empty;
        }

        public bool ResetUserPassword(ResetPasswordModel rpm)
        {
            if(!string.IsNullOrEmpty(rpm.Email) && !string.IsNullOrEmpty(rpm.Password))
            {
                using (var _ctx = new ChinmayaEntities())
                {
                    if(IsEmailExists(rpm.Email))
                    {
                        var objUser = _ctx.Users.FirstOrDefault(x => x.Email == rpm.Email);
                        objUser.Password = rpm.Password;
                        _ctx.Entry(objUser).State = System.Data.Entity.EntityState.Modified;
                        _ctx.SaveChanges();
                        return true;
                    }

                }
            }
            return false;
        }

        /*public List<KeyValueModel> GetGender()
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Genders.Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

		public List<KeyValueModel> GetCountryList()
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Countries.Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

		public List<KeyValueModel> GetStateList(int CountryId)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.States.Where(s => s.Id == CountryId).Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}

		public List<KeyValueModel> GetCityList(int StateId)
		{
			using (var _ctx = new ChinmayaEntities())
			{
				return _ctx.Cities.Where(c => c.Id == StateId).Select(x => new KeyValueModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();
			}
		}*/

    }
}

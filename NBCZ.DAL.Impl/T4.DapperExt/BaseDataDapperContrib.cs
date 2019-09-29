﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
//using DapperExtensions;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using System.Data.SqlClient;
using System.Text;
using NBCZ.Model;
using System.Data;
using NBCZ.DAL.Interface;

namespace NBCZ.DAL.Impl
{
    public partial class BaseDataDapperContrib<T>:IBaseData<T> where T : class ,new()
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long Insert(T model)
        {
            dynamic r = null;
            using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
            {
                cn.Open();
                r = cn.Insert(model);
                cn.Close();
            }

            return r;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool InsertBatch(List<T> models)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
                {
                    cn.Open();
                    foreach (var model in models)
                    {
                        cn.Insert(model);
                    }
                    cn.Close();
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }



        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(T model)
        {
            dynamic r = null;
            using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
            {
                cn.Open();
                r = cn.Update(model);
                cn.Close();
            }

            return r;
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool UpdateBatch(List<T> models)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
                {
                    cn.Open();
                    foreach (var model in models)
                    {
                        cn.Update(model);
                    }
                    cn.Close();
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


        /// <summary>
        ///根据实体删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public dynamic Delete(T model)
        {
            dynamic r = null;
            using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
            {
                cn.Open();
                r = cn.Delete(model);
                cn.Close();
            }

            return r;
        }


        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public dynamic Delete(object predicate)
        {
            dynamic r = null;
            using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
            {
                cn.Open();
                r = cn.Delete(predicate);
                cn.Close();
            }

            return r;
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteByWhere(string where, object param = null)
        {
            try
            {
                var tableName = typeof(T).Name;
                StringBuilder sql = new StringBuilder().AppendFormat(" Delete FROM {0} ", tableName);
                if (string.IsNullOrEmpty(where))
                {
                    return false;
                }

                sql.AppendFormat(" where {0} ", where);
                using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
                {
                    cn.Execute(sql.ToString(), param);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
          
        }
        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool DeleteBatch(List<T> models)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
                {
                    cn.Open();
                    foreach (var model in models)
                    {
                        var r = cn.Delete(model);
                    }
                    cn.Close();
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


        /// <summary>
        /// 根据一个实体对象
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public T Get(object id)
        {
            T t = default(T); //默认只对int guid主键有作用除非使用ClassMapper
            using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
            {
                cn.Open();
                t = cn.Get<T>(id);
                cn.Close();
            }

            return t;

        }

        /// <summary>
        /// 获取一个实体对象
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public T Get(object id, string keyName)
        {
            var tableName = typeof(T).Name;
            StringBuilder sql = new StringBuilder().AppendFormat("SELECT  TOP 1 * FROM {0} WHERE {1}=@id ", tableName, keyName);
            var pms = new { id = id };
            using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
            {
                return cn.Query<T>(sql.ToString(), pms).FirstOrDefault();
            }

        }

      

        /// <summary>
        /// 根据条件查询实体列表
        /// </summary>
        /// <param name="where"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public List<T> GetList(string where, string sort = null, int limits = -1, string fileds = " * ", string orderby = "")
        {
            var tableName = typeof(T).Name;
            StringBuilder sql = new StringBuilder().AppendFormat("SELECT " + (limits > 0 ? (" TOP " + limits) : " ") + fileds + "  FROM {0} {1} ",
                tableName, (string.IsNullOrWhiteSpace(orderby) ? "" : (" order by " + orderby)));
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where {0} ", where);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                sql.AppendFormat(" order by {0} ", sort);
            }

            using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
            {
                return cn.Query<T>(sql.ToString()).ToList();
            }

        }





        /// <summary>
        /// 存储过程分页查询
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="sort">分类</param>
        /// <param name="page">页索引</param>
        /// <param name="resultsPerPage">页大小</param>
        /// <param name="fields">查询字段</param>
        /// <returns></returns>
        public PageDateRes<T> GetPage(string where, string sort, int page, int resultsPerPage, string fields = "*", Type result = null)
        {
            var tableName = typeof(T).Name;
            var p = new DynamicParameters();
            p.Add("@TableName", tableName);
            p.Add("@Fields", fields);
            p.Add("@OrderField", sort);
            p.Add("@sqlWhere", where);
            p.Add("@pageSize", resultsPerPage);
            p.Add("@pageIndex", page);
            p.Add("@TotalPage", 0, direction: ParameterDirection.Output);
            p.Add("@Totalrow", 0, direction: ParameterDirection.Output);

            using (SqlConnection cn = new SqlConnection(DapperHelper.ConnStr))
            {

                var data = cn.Query<T>("P_ZGrid_PagingLarge", p, commandType: CommandType.StoredProcedure, commandTimeout: 120);
                int totalPage = p.Get<int>("@TotalPage");
                int totalrow = p.Get<int>("@Totalrow");

                var rep = new PageDateRes<T>()
                {
                    code =ResCode.Success,
                    count = totalrow,
                    totalPage = totalPage,
                    data = data.ToList(),
                    PageNum = page,
                    PageSize = resultsPerPage
                };

                return rep;
            }
        }

        /// <summary>
        /// 修改删除状态
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        
        public bool ChangeSotpStatus(string where)
        {
            var tableName = typeof(T).Name;
            string sql = "UPDATE "+tableName+" SET StopFlag =1 ";
            if (string.IsNullOrWhiteSpace(where))
            {
                return false;
            }

            sql += " where " + where;

            return DapperHelper.Excute(sql) > 0;
        }
    }
}
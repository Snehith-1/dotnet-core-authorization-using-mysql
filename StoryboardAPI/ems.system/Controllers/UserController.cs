using Microsoft.AspNetCore.Mvc;
using System.Data;
using ems.system.Models;
using ems.utilities.Functions;

namespace ems.system.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        string? msSQL, menu_ind_up_second, menu_ind_down_second, menu_ind_down_first, menu_ind_up_first = string.Empty;
        DataTable? dt_levelone;
        private readonly dbconn objdbconn;

        public UserController(dbconn dbconn)
        {
            objdbconn = dbconn;
        }
        
        [HttpGet("topmenu")]
        public IActionResult getTopMenu(string user_gid)
        {
            menu_response objresult = new menu_response();
            var dt_data = new DataTable();
            List<sys_menu> getmenu = new List<sys_menu>();
            List<mdlMenuData> mdlMenuData = new List<mdlMenuData>();

            msSQL = " CALL adm_mst_spGetMenuDataangular('" + user_gid + "')";
            dt_levelone = objdbconn.GetDataTable(msSQL);
            if (dt_levelone != null)
            {
                mdlMenuData = cmnfunctions.ConvertDataTable<mdlMenuData>(dt_levelone);
                try
                {
                    List<mdlMenuData> getFirstLevel = mdlMenuData.Where(a => a.menu_level == "1").ToList();
                    if (getFirstLevel.Count != 0)
                    {
                        foreach (var i in getFirstLevel)
                        {
                            List<mdlMenuData> getSecondLevel = mdlMenuData.Where(a => a.menu_level == "2"
                                   && a.module_gid_parent == i.module_gid).OrderBy(a => a.display_order).GroupBy(a => a.module_gid)
                                   .Select(group => new mdlMenuData
                                   {
                                       module_gid = group.Key,
                                       module_name = group.First().module_name,
                                       sref = group.First().sref,
                                       icon = group.First().icon,
                                       menu_level = group.First().menu_level,
                                       module_gid_parent = group.First().module_gid_parent,
                                       display_order = group.First().display_order
                                   }).ToList();
                            List<sys_submenu> getmenu2 = new List<sys_submenu>();
                            if (getSecondLevel != null)
                            {
                                foreach (var j in getSecondLevel)
                                {
                                    List<mdlMenuData> getThirdLevel = mdlMenuData.Where(a => a.menu_level == "3" && a.sref != null && a.sref != ""
                                    && a.module_gid_parent == j.module_gid).OrderBy(a => a.display_order).GroupBy(a => a.module_gid)
                                    .Select(group => new mdlMenuData
                                    {
                                        module_gid = group.Key,
                                        module_name = group.First().module_name,
                                        sref = group.First().sref,
                                        icon = group.First().icon,
                                        menu_level = group.First().menu_level,
                                        module_gid_parent = group.First().module_gid_parent,
                                        display_order = group.First().display_order
                                    }).ToList();
                                    List<sys_sub1menu> getmenu3 = new List<sys_sub1menu>();
                                    if (getThirdLevel != null)
                                    {
                                        foreach (var k in getThirdLevel)
                                        {
                                            var getFourthLevel = mdlMenuData.Where(a => a.menu_level == "4"
                                                                 && a.module_gid_parent == k.module_gid)
                                                                 .OrderBy(a => a.display_order)
                                                                 .GroupBy(a => a.module_gid).ToList();
                                            List<sys_sub2menu> getmenu4 = new List<sys_sub2menu>();
                                            if (getFourthLevel != null)
                                            {
                                                menu_ind_up_second = "fa fa-angle-up";
                                                menu_ind_down_second = "fa fa-angle-down";
                                                getmenu4 = getFourthLevel.SelectMany(group => group).Select(row => new sys_sub2menu
                                                {
                                                    text = row.module_name,
                                                    sref = row.sref,
                                                    icon = row.icon,
                                                }).ToList();
                                            }
                                            getmenu3.Add(new sys_sub1menu
                                            {
                                                text = k.module_name,
                                                sref = k.sref,
                                                sub2menu = getmenu4,
                                            });
                                        }
                                    }
                                    getmenu2.Add(new sys_submenu
                                    {
                                        text = j.module_name,
                                        sref = j.sref,
                                        sub1menu = getmenu3
                                    });
                                }
                            }
                            else
                            {
                                menu_ind_up_first = "";
                                menu_ind_down_first = "";
                            }
                            getmenu.Add(new sys_menu
                            {
                                text = i.module_name,
                                sref = i.sref,
                                icon = i.icon,
                                menu_indication = menu_ind_up_first,
                                menu_indication1 = menu_ind_down_first,
                                label = "label label-success",
                                submenu = getmenu2
                            });
                            objresult.menu_list = getmenu;
                        }
                    }
                }
                catch (Exception ex)
                {
                    objresult.message = ex.ToString();
                    objresult.status = false;
                }
                finally
                {
                }
                dt_levelone.Dispose();
                objresult.status = true;
               
            }
            return Ok(objresult);
        }
    }
}

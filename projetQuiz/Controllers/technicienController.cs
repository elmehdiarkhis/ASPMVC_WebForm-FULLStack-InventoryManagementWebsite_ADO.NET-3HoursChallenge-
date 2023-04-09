using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

using projetQuiz.Models;

namespace projetQuiz.Controllers
{
    public class technicienController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            ViewBag.loginFail = "";
            return View("Login");
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(User c)
        {
            string username = c.username.ToString();
            string pass = c.pass.ToString();


            string viewName = "";

            //check if existe in technicienDb _ Login table 
            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                string sql = "SELECT UserId FROM LOGIN WHERE UserName='" + username + "' AND Pass='" + pass + "'";

                //string sql = "SELECT UserId FROM LOGIN WHERE UserName=@username AND Pass=@pass";

                SqlCommand myComm = new SqlCommand(sql, myCon);

                //myComm.Parameters.Add("@username", SqlDbType.NChar);
                //myComm.Parameters["@username"].Value = username;

                //myComm.Parameters.Add("@pass", SqlDbType.NChar);
                //myComm.Parameters["@pass"].Value = pass;

                //faire les requette parametrer

                SqlDataReader myReader = myComm.ExecuteReader();
                if (myReader.Read() == false)
                {
                    ViewBag.loginFail = "incorrect userName or password";
                    viewName = "login";
                }
                else
                {
                    GlobalVar.userId = Convert.ToInt32(myReader["UserId"]);

                    return RedirectToAction("EspaceUser");
                }
                myReader.Close();
            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }


            return View(viewName);
        }



        // GET: EspaceUser
        public ActionResult EspaceUser()
        {
            RemplireSelect();




            List<Equipement> myEquipementList = new List<Equipement>();

            Int32 userId = GlobalVar.userId;


            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                string sqlSelect = "SELECT * FROM Equipement;";
                SqlCommand myCommSelect = new SqlCommand(sqlSelect, myCon);

                SqlDataReader myReaderSelect = myCommSelect.ExecuteReader();
                while (myReaderSelect.Read())
                {
                    myEquipementList.Add(new Equipement
                    {
                        EquipementId = Convert.ToInt32(myReaderSelect["EquipementId"].ToString().Trim()),
                        numSerie = Convert.ToInt32(myReaderSelect["numSerie"].ToString().Trim()),
                        nom = myReaderSelect["nom"].ToString().Trim(),
                        type = myReaderSelect["type"].ToString().Trim(),
                        prix = Convert.ToInt32(myReaderSelect["prix"].ToString().Trim()),
                        description = myReaderSelect["description"].ToString().Trim(),
                    });
                }


                myReaderSelect.Close();


            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }

            return View("EspaceUser", myEquipementList);
        }


        // GET: Create
        public ActionResult Create()
        {
            GlobalVar.updateClicked = false;
            ViewBag.act = "Create";

            ViewBag.exist = "";



            RemplireSelect();

            return View("Create");


        }



        // POST: Create
        [HttpPost]
        public ActionResult Create(Equipement e)
        {

            int numSerie = e.numSerie;
            string nom = e.nom;
            string type = e.type;
            int prix = e.prix;
            string description = e.description;


            string viewName = "";

            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                string sql = "SELECT EquipementId FROM Equipement WHERE NumSerie=" + numSerie + ";";
                SqlCommand myComm = new SqlCommand(sql, myCon);


                SqlDataReader myReader = myComm.ExecuteReader();
                if (myReader.Read())
                {
                    //RemplirSelect();
                    ViewBag.exist = "Ce Numero de Serie est deja associee a un Equipement";
                    myReader.Close();
                    RemplireSelect();
                    return View("Create");
                }
                else
                {
                    myReader.Close();

                    // Insert ============
                    string sqlInsert = "INSERT INTO Equipement(NumSerie,Nom,Type,Prix,Description) VALUES(" + numSerie + ",'" + nom + "','" + type + "'," + prix + ",'" + description + "');";
                    SqlCommand myCommInsert = new SqlCommand(sqlInsert, myCon);
                    myCommInsert.ExecuteNonQuery();

                    viewName = "EspaceUser";

                }
            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }


            //checker le Closing des Reader
            return RedirectToAction(viewName);

        }




        //Get : Delete
        public ActionResult Delete(int id)
        {
            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                // Insert ============
                string sqlDelete = "DELETE FROM Equipement WHERE EquipementId=" + id + ";";
                SqlCommand myCommDelete = new SqlCommand(sqlDelete, myCon);
                myCommDelete.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }

            return RedirectToAction("EspaceUser");
        }



        //Get : Update
        public ActionResult Update(int id)
        {
            GlobalVar.updateClicked = true;
            ViewBag.act = "Update";


            RemplireSelect();


            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                string sql = "SELECT * FROM Equipement WHERE EquipementId=" + id + ";";
                SqlCommand myComm = new SqlCommand(sql, myCon);


                SqlDataReader myReader = myComm.ExecuteReader();
                if (myReader.Read())
                {
                    Equipement e = new Equipement
                    {
                        EquipementId = Convert.ToInt32(myReader["EquipementId"].ToString().Trim()),
                        numSerie = Convert.ToInt32(myReader["numSerie"].ToString().Trim()),
                        nom = myReader["nom"].ToString().Trim(),
                        type = myReader["type"].ToString().Trim(),
                        prix = Convert.ToInt32(myReader["prix"].ToString().Trim()),
                        description = myReader["description"].ToString().Trim()
                    };

                    myReader.Close();

                    return View("Create", e);

                }



            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {

                myCon.Close();
            }

            return View("Create");



        }

        [HttpPost]
        public ActionResult Update(Equipement e)
        {
            int id = e.EquipementId;
            int numSerie = e.numSerie;
            string nom = e.nom;
            string type = e.type;
            int prix = e.prix;
            string description = e.description;


            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                // Update ============
                string sqlUpdate = "UPDATE Equipement SET NumSerie='"+numSerie+"',Nom='"+nom+"',Type='"+type+"',Prix="+prix+",Description='"+description+ "' WHERE EquipementId="+id+";";
                SqlCommand myCommUpdate = new SqlCommand(sqlUpdate, myCon);
                myCommUpdate.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }



            //checker le Closing des Reader
            return RedirectToAction("EspaceUser");
        }

        public ActionResult sortByPrice()
        {

            RemplireSelect();

            List<Equipement> myEquipementList = new List<Equipement>();


            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();
                string sqlSelect="";
                if (GlobalVar.prixClicked == false)
                {
                    if(GlobalVar.typeClicked == false)
                    {
                        sqlSelect = "SELECT * FROM Equipement ORDER BY Prix ASC;";
                    }
                    else
                    {
                        sqlSelect = "SELECT * FROM Equipement WHERE Type='"+GlobalVar.choosenType+"'  ORDER BY Prix ASC;";
                    }
                    
                }
                else
                {
                    if (GlobalVar.typeClicked == false)
                    {
                        sqlSelect = "SELECT * FROM Equipement ORDER BY Prix DESC;";
                    }
                    else
                    {
                        sqlSelect = "SELECT * FROM Equipement WHERE Type='" + GlobalVar.choosenType + "'  ORDER BY Prix DESC;";
                    }
                }
                
                SqlCommand myCommSelect = new SqlCommand(sqlSelect, myCon);

                SqlDataReader myReaderSelect = myCommSelect.ExecuteReader();
                while (myReaderSelect.Read())
                {
                    myEquipementList.Add(new Equipement
                    {
                        EquipementId = Convert.ToInt32(myReaderSelect["EquipementId"].ToString().Trim()),
                        numSerie = Convert.ToInt32(myReaderSelect["numSerie"].ToString().Trim()),
                        nom = myReaderSelect["nom"].ToString().Trim(),
                        type = myReaderSelect["type"].ToString().Trim(),
                        prix = Convert.ToInt32(myReaderSelect["prix"].ToString().Trim()),
                        description = myReaderSelect["description"].ToString().Trim(),
                    });
                }


                myReaderSelect.Close();

                if (GlobalVar.prixClicked == false)
                {
                    GlobalVar.prixClicked = true;
                }
                else
                {
                    GlobalVar.prixClicked = false;
                }
            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }

            return View("EspaceUser", myEquipementList);

        }


        public ActionResult sortById()
        {
            RemplireSelect();

            List<Equipement> myEquipementList = new List<Equipement>();


            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();
                string sqlSelect = "";
                if (GlobalVar.idClicked == false)
                {
                    if (GlobalVar.typeClicked == false)
                    {
                        sqlSelect = "SELECT * FROM Equipement ORDER BY EquipementId ASC;";
                    }
                    else
                    {
                        sqlSelect = "SELECT * FROM Equipement WHERE Type='"+GlobalVar.choosenType+"' ORDER BY EquipementId ASC;";
                    }
                }
                else
                {            
                    if (GlobalVar.typeClicked == false)
                    {
                        sqlSelect = "SELECT * FROM Equipement ORDER BY EquipementId DESC;";
                    }
                    else
                    {
                        sqlSelect = "SELECT * FROM Equipement WHERE Type='"+GlobalVar.choosenType+"' ORDER BY EquipementId DESC;";
                    }
                }

                SqlCommand myCommSelect = new SqlCommand(sqlSelect, myCon);

                SqlDataReader myReaderSelect = myCommSelect.ExecuteReader();
                while (myReaderSelect.Read())
                {
                    myEquipementList.Add(new Equipement
                    {
                        EquipementId = Convert.ToInt32(myReaderSelect["EquipementId"].ToString().Trim()),
                        numSerie = Convert.ToInt32(myReaderSelect["numSerie"].ToString().Trim()),
                        nom = myReaderSelect["nom"].ToString().Trim(),
                        type = myReaderSelect["type"].ToString().Trim(),
                        prix = Convert.ToInt32(myReaderSelect["prix"].ToString().Trim()),
                        description = myReaderSelect["description"].ToString().Trim(),
                    });
                }


                myReaderSelect.Close();

                if (GlobalVar.idClicked == false)
                {
                    GlobalVar.idClicked = true;
                }
                else
                {
                    GlobalVar.idClicked = false;
                }
            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }

            return View("EspaceUser", myEquipementList);

        }


        public ActionResult sortByType(string type)
        {
            GlobalVar.typeClicked = true;
            GlobalVar.choosenType = type;


            RemplireSelect();



            List<Equipement> myEquipementList = new List<Equipement>();


            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon.Open();
                string sqlSelect = "SELECT * FROM Equipement WHERE Type='"+type+"';";
                

                SqlCommand myCommSelect = new SqlCommand(sqlSelect, myCon);

                SqlDataReader myReaderSelect = myCommSelect.ExecuteReader();
                while (myReaderSelect.Read())
                {
                    myEquipementList.Add(new Equipement
                    {
                        EquipementId = Convert.ToInt32(myReaderSelect["EquipementId"].ToString().Trim()),
                        numSerie = Convert.ToInt32(myReaderSelect["numSerie"].ToString().Trim()),
                        nom = myReaderSelect["nom"].ToString().Trim(),
                        type = myReaderSelect["type"].ToString().Trim(),
                        prix = Convert.ToInt32(myReaderSelect["prix"].ToString().Trim()),
                        description = myReaderSelect["description"].ToString().Trim(),
                    });
                }


                myReaderSelect.Close();

            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }

            return View("EspaceUser", myEquipementList);

        }

        public ActionResult Reset()
        {
            GlobalVar.typeClicked = false;
            GlobalVar.choosenType = "";
            return RedirectToAction("EspaceUser");
        }



        public void RemplireSelect()
        {
            List<string> myTypesList = new List<string>();

            SqlConnection myCon_1 = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\projetQuiz\projetQuiz\App_Data\technicienDB.mdf;Integrated Security=True");
            try
            {
                myCon_1.Open();

                string sqlSelect = "SELECT * FROM TYPES;";
                SqlCommand myCommSelect = new SqlCommand(sqlSelect, myCon_1);

                SqlDataReader myReaderSelect = myCommSelect.ExecuteReader();
                while (myReaderSelect.Read())
                {
                    myTypesList.Add(myReaderSelect["nomType"].ToString().Trim());
                }


                myReaderSelect.Close();


            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon_1.Close();
            }

            ViewBag.listOfTypes = myTypesList;
        }


        




    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw2.DTOs.Requests;
using cw2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw2.Controllers
{
    [Route("api/enrollment")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        string conString = "Data Source=db-mssql;Initial Catalog=s14958;Integrated Security=True";

        [HttpPost]
        public async Task<IActionResult> EnrollStudent(EnrollmentStudentRequest request)
        {   
            var student = new Student
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IndexNumber = request.IndexNumber
            };

            using (var connect = new SqlConnection(conString))
            using (var command = new SqlCommand())
            {
                command.Connection = connect;

                connect.Open();

                var tran = connect.BeginTransaction();
                command.Transaction = tran;

                try
                {
                    // 1. Czy studia istnieją?
                    command.CommandText = "select IdStudy from Studies where name=@name";
                    command.Parameters.AddWithValue("name", request.Studies);

                    using(var dr = command.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            tran.Rollback();
                            return BadRequest("Studia nie istnieją");
                        }

                        int idStudies = (int)dr["IdStudy"];

                        tran.Commit();
                    }

                    

                    // x. dodanie studenta
                    //command.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate) VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate)";
                    //command.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                    //command.Parameters.AddWithValue("FirstName", request.FirstName);
                    //command.Parameters.AddWithValue("LastName", request.LastName);
                    //command.Parameters.AddWithValue("BirthDate", request.BirthDate);

                    //command.ExecuteNonQuery();

                    
                }
                catch (SqlException exc)
                {
                    tran.Rollback();
                    return BadRequest("Error: " + exc.Message);
                }
                return Ok("Student enrolled");
                //var enrollment = new Enrollment();
                //return CreatedAtAction("enroll", enrollment);
            }
        }
    }
}
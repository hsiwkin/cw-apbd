using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw2.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        string conString = "Data Source=db-mssql;Initial Catalog=s14958;Integrated Security=True";

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var list = new List<StudentInfoDTO>();
            using (SqlConnection con = new SqlConnection(conString)) 
            using(SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select s.FirstName, s.LastName, s.BirthDate, st.Name, e.Semester from Student s join Enrollment e on e.IdEnrollment = s.IdEnrollment join Studies st on st.IdStudy = e.IdStudy";

                con.Open();

                using(SqlDataReader dr = com.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var st = new StudentInfoDTO
                        {
                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                            BirthDate = dr["BirthDate"].ToString(),
                            Name = dr["Name"].ToString(),
                            Semester = dr["Semester"].ToString()
                        };
                        list.Add(st);
                    }
                }
            }
            return Ok(list);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent([FromRoute] string indexNumber)
        {
            var st = new StudentInfoDTO();
            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = $"select s.FirstName, s.LastName, s.BirthDate, st.Name, e.Semester from Student s join Enrollment e on e.IdEnrollment = s.IdEnrollment join Studies st on st.IdStudy = e.IdStudy where s.IndexNumber=@indexNumber";
                com.Parameters.AddWithValue("indexNumber", indexNumber);
                con.Open();

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        st.FirstName = dr["FirstName"].ToString();
                        st.LastName = dr["LastName"].ToString();
                        st.BirthDate = dr["BirthDate"].ToString();
                        st.Name = dr["Name"].ToString();
                        st.Semester = dr["Semester"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            return Ok(st);
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            student.IdStudent = id;
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok("Aktualizacja dokonczona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukoczone");
        }
    }
}
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Json;
using DisprzTraining.Data;

namespace DisprzTraining.Tests
{
    public class AppointmentServiceIntegratedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpclient;

        public AppointmentServiceIntegratedTest()
        {
            var sut = new WebApplicationFactory<Program>();
            _httpclient = sut.CreateClient();
        }
        public string BASEURL = "api/appointments";
        private void CleanUp()
        {
            DataStore.dictionaryData.Clear();
        }
        [Fact]

        public async Task Create_Appointment_Returns_Success_Code()
        {
            //Arrange
            
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 31),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 2, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 3, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            //Act
            var response = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var appointment = JsonConvert.DeserializeObject<bool>(content);

            var response_2 = await _httpclient.GetAsync($"{BASEURL}/2023-01-31T00:00:00");

            response_2.EnsureSuccessStatusCode();

            var content_2 = response_2.Content.ReadFromJsonAsync<List<Appointment>>();

            var getId = content_2?.Result[0].Id;

            //Assert
            Assert.True(content_2?.Result?.Count > 0);
            Assert.Equal(HttpStatusCode.OK, response_2.StatusCode);
            Assert.True(appointment);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var deleteResponse = await _httpclient.DeleteAsync($"{BASEURL}/{getId}/2023-01-31T00:00:00");
        }

        [Fact]

        public async Task Create_Appointment_Returns_Conflict()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 31),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 2, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 3, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            //time conflicts
            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 2, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 3, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            //Act
            var response = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var appointment = JsonConvert.DeserializeObject<bool>(content);

            var postResponse = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_2));

            var response_2 = await _httpclient.GetAsync($"{BASEURL}/2023-01-31T00:00:00");

            var content_2 = response_2.Content.ReadFromJsonAsync<List<Appointment>>();

            var getId = content_2?.Result[0].Id;

            var deleteResponse = await _httpclient.DeleteAsync($"{BASEURL}/{getId}/2023-01-31T00:00:00");
            //Assert
            Assert.Equal(HttpStatusCode.Conflict, postResponse.StatusCode);
        }

        [Fact]

        public async Task Create_Appointment_Bad_Request()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 31),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 2, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 3, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            //start-time and end-time same
            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 7, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 7, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };

            //start time greater than end time
            AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 7, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 6, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            //invalid Attachment
             AddNewAppointment data_4 = new AddNewAppointment()
            {
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 7, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 8, 30, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = MockDatas.dummyBase64,
                        ContentName = "Calender",
                        ContentType = "image",
                    }
            };


            AddNewAppointment data_5 = new AddNewAppointment()
            {
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 7, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 7, 30, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = MockDatas.base64,
                        ContentName = " ",
                        ContentType = " ",
                    }
            };
            //Act
            var response = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var appointment = JsonConvert.DeserializeObject<bool>(content);

            var postResponse = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_2));

            var postResponse_2 = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_3));
            
            var postResponse_3= await _httpclient.PostAsync(BASEURL,TestHelper.GetStringContent(data_4));

            var postResponse_4= await _httpclient.PostAsync(BASEURL,TestHelper.GetStringContent(data_5));

            var response_2 = await _httpclient.GetAsync($"{BASEURL}/2023-01-31T00:00:00");

            var content_2 = response_2.Content.ReadFromJsonAsync<List<Appointment>>();

            var getId = content_2?.Result[0].Id;
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, postResponse_2.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, postResponse_3.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, postResponse_4.StatusCode);

            var deleteResponse = await _httpclient.DeleteAsync($"{BASEURL}/{getId}/2023-01-31T00:00:00");

        }

        [Fact]
        public async Task Update_Appointment_Returns_Ok_Result()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 31),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 2, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 3, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };

            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 5, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 6, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };

            //Act
            var response = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var appointment = JsonConvert.DeserializeObject<bool>(content);

            var postResponse = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_2));

            var response_2 = await _httpclient.GetAsync($"{BASEURL}/2023-01-31T00:00:00");

            var content_2 = response_2.Content.ReadFromJsonAsync<List<Appointment>>();

            var getId = content_2.Result[0].Id;
            UpdateAppointment updateData = new UpdateAppointment()
            {
                Appointment=new Appointment()
            {
                Id = content_2.Result[1].Id,
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 8, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 9, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            },
            OldDate=data.Date};
            var updateResponse = await _httpclient.PutAsync(BASEURL, TestHelper.GetStringContent(updateData));

            //Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.Equal(2,content_2.Result.Count);
            await _httpclient.DeleteAsync($"{BASEURL}/{content_2.Result[1].Id}/2023-01-31T00:00:00");
            var deleteResponse = await _httpclient.DeleteAsync($"{BASEURL}/{getId}/2023-01-31T00:00:00");
        }

        [Fact]
        public async Task Update_Appointment_Returns_Conflict()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 31),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 2, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 3, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };

            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 5, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 6, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = new DateTime(2023,02,01),
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 02, 01, 2, 0, 0),
                EndTime = new DateTime(2023, 02, 01, 3, 30, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            //Act
            var response = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data));
            
            var postResponse = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_2));

            var postResponse_2= await _httpclient.PostAsync(BASEURL,TestHelper.GetStringContent(data_3));

            var response_2 = await _httpclient.GetAsync($"{BASEURL}/2023-01-31T00:00:00");

            var response_3= await _httpclient.GetAsync($"{BASEURL}/2023-02-01T00:00:00");

            var content_2 = response_2.Content.ReadFromJsonAsync<List<Appointment>>();

            var content_3= response_3.Content.ReadFromJsonAsync<List<Appointment>>();

            var getId = content_2?.Result[0].Id;
            UpdateAppointment updateData = new UpdateAppointment()
            {
                Appointment=new Appointment()
                {
                Id = content_2.Result[1].Id,
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 2, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 3, 30, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            },
            OldDate=data.Date
            };
            var updateResponse = await _httpclient.PutAsync(BASEURL, TestHelper.GetStringContent(updateData));
             UpdateAppointment updateData_2 = new UpdateAppointment()
            {
                Appointment=new Appointment()
                {
                Id = content_2.Result[1].Id,
                Date = new DateTime(2023,02,01),
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 02, 01, 1, 0, 0),
                EndTime = new DateTime(2023, 02, 01, 2, 30, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            },
            OldDate=data.Date
            };

            var updateResponse_2 = await _httpclient.PutAsync(BASEURL, TestHelper.GetStringContent(updateData_2));
            //Assert
            Assert.Equal(HttpStatusCode.Conflict, updateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict,updateResponse_2.StatusCode);
            await _httpclient.DeleteAsync($"{BASEURL}/{content_2.Result[1].Id}/2023-01-31T00:00:00");
            var deleteResponse = await _httpclient.DeleteAsync($"{BASEURL}/{getId}/2023-01-31T00:00:00");
            await _httpclient.DeleteAsync($"{BASEURL}/{content_3.Result[0].Id}/2023-01-31T00:00:00");

        }

        [Fact]
        public async Task Update_Appointment_Returns_Bad_Request()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 31),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 2, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 3, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };

            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 5, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 6, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
             AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = new DateTime(2023,02,01),
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 02, 01, 2, 0, 0),
                EndTime = new DateTime(2023, 02, 01, 3, 30, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };

            //Act
            var response = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var appointment = JsonConvert.DeserializeObject<bool>(content);

            var postResponse = await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_2));

            var postResponse_2= await _httpclient.PostAsync(BASEURL,TestHelper.GetStringContent(data_3));

            var response_2 = await _httpclient.GetAsync($"{BASEURL}/2023-01-31T00:00:00");

            var response_3= await _httpclient.GetAsync($"{BASEURL}/2023-02-01T00:00:00");

            var content_2 = response_2.Content.ReadFromJsonAsync<List<Appointment>>();

            var content_3= response_3.Content.ReadFromJsonAsync<List<Appointment>>();

            var getId = content_2.Result[0].Id;
            UpdateAppointment updateData = new UpdateAppointment()
            {
             Appointment= new Appointment()
            {
                Id = content_2.Result[1].Id,
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 7, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 7, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            },
            OldDate=data.Date,
            };

            UpdateAppointment updateData_2 = new UpdateAppointment()
            {
             Appointment=new Appointment()
            {
                Id = content_2.Result[1].Id,
                Date = data.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 7, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 6, 30, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            },
            OldDate=data.Date
            };
            //-id is wrong
              UpdateAppointment updateData_3 = new UpdateAppointment()
            {
                Appointment=new Appointment()
                {
                Id = new Guid("eaa24756-3fac-4e46-b4bb-074ff4f6b864"),
                Date = new DateTime(2023,02,01),
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 02, 01, 1, 0, 0),
                EndTime = new DateTime(2023, 02, 01, 2, 30, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            },
            OldDate=data.Date
            };
            var updateResponse = await _httpclient.PutAsync(BASEURL, TestHelper.GetStringContent(updateData));
            var updateResponse_2 = await _httpclient.PutAsync(BASEURL, TestHelper.GetStringContent(updateData_2));
            var updateResponse_3=await _httpclient.PutAsync(BASEURL,TestHelper.GetStringContent(updateData_3));
            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, updateResponse_2.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, updateResponse_3.StatusCode);
            await _httpclient.DeleteAsync($"{BASEURL}/{content_2.Result[1].Id}/2023-01-31T00:00:00");
            var deleteResponse = await _httpclient.DeleteAsync($"{BASEURL}/{getId}/2023-01-31T00:00:00");
            await _httpclient.DeleteAsync($"{BASEURL}/{content_3?.Result[0].Id}/2023-02-01T00:00:00");
        }

        [Fact]
        public async Task Get_List_By_Range()
        {
            //Arrange
            AddNewAppointment data_1 = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 26),
                Title = "test",
                Description = "test-case",
                Type = "reminder",
                StartTime = new DateTime(2023, 01, 26, 11, 0, 0),
                EndTime = new DateTime(2023, 01, 26, 12, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 27),
                Title = "test",
                Description = "test-case",
                Type = "reminder",
                StartTime = new DateTime(2023, 01, 27, 11, 0, 0),
                EndTime = new DateTime(2023, 01, 27, 12, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 27),
                Title = "test",
                Description = "test-case",
                Type = "reminder",
                StartTime = new DateTime(2023, 01, 27, 13, 0, 0),
                EndTime = new DateTime(2023, 01, 27, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            AddNewAppointment data_4 = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 28),
                Title = "test",
                Description = "test-case",
                Type = "reminder",
                StartTime = new DateTime(2023, 01, 28, 13, 0, 0),
                EndTime = new DateTime(2023, 01, 28, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            AddNewAppointment data_5 = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 29),
                Title = "test",
                Description = "test-case",
                Type = "reminder",
                StartTime = new DateTime(2023, 01, 29, 13, 0, 0),
                EndTime = new DateTime(2023, 01, 29, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            AddNewAppointment data_6 = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 30),
                Title = "test",
                Description = "test-case",
                Type = "reminder",
                StartTime = new DateTime(2023, 01, 30, 13, 0, 0),
                EndTime = new DateTime(2023, 01, 30, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };
            AddNewAppointment data_7 = new AddNewAppointment()
            {
                Date = new DateTime(2023, 02, 03),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 02, 03, 13, 0, 0),
                EndTime = new DateTime(2023, 02, 03, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
            };

            // DateTime date = new DateTime(2023, 01, 26);

            //Act
            await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_1));
            await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_2));
            await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_3));
            await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_4));
            await _httpclient.PostAsync(BASEURL, TestHelper.GetStringContent(data_5));
            await _httpclient.PostAsync(BASEURL,TestHelper.GetStringContent(data_6));
            await _httpclient.PostAsync(BASEURL,TestHelper.GetStringContent(data_7));

            var response_2 = await _httpclient.GetAsync($"{BASEURL}/range/2023-01-26T00:00:00");
            var content_2 = response_2.Content.ReadFromJsonAsync<List<Appointment>>();

            //Assert
            await _httpclient.DeleteAsync($"{BASEURL}/{content_2.Result[0].Id}/2023-01-31T00:00:00");
            await _httpclient.DeleteAsync($"{BASEURL}/{content_2.Result[0].Id}/2023-01-31T00:00:00");
            await _httpclient.DeleteAsync($"{BASEURL}/{content_2.Result[0].Id}/2023-01-31T00:00:00");
            await _httpclient.DeleteAsync($"{BASEURL}/{content_2.Result[0].Id}/2023-01-31T00:00:00");
            await _httpclient.DeleteAsync($"{BASEURL}/{content_2.Result[0].Id}/2023-01-31T00:00:00");
            Assert.Equal(HttpStatusCode.OK,response_2.StatusCode);
            Assert.Equal(6,content_2?.Result?.Count);
            // CleanUp();

        }
    }
}
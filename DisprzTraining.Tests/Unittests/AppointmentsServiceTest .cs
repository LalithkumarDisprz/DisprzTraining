


namespace DisprzTraining.Tests
{
    public class AppointmentsServiceTest
    {
        private readonly IAppointmentsDAL _appointmentDAL;
        private readonly IAppointmentsBL _appointmentBL;
        private readonly AppointmentsController _appointment;
        public AppointmentsServiceTest()
        {
            _appointmentDAL = new AppointmentsDAL();
            _appointmentBL = new AppointmentsBL(_appointmentDAL);
            _appointment = new AppointmentsController(_appointmentBL);
        }
        public static AddNewAppointment testData = new AddNewAppointment()
        {
            Date = DateTime.Now,
            Title = "test",
            Description = "test-case",
            Type = "Reminder",
            StartTime = DateTime.Now.AddMinutes(30),
            EndTime = DateTime.Now.AddHours(1),
            AppointmentAttachment = new Attachment()
            {
                Content = "",
                ContentName = "",
                ContentType = "",
            }
        };
        private List<Appointment> GetTestData(ActionResult<List<Appointment>> getResult)
        {
            var resData = (List<Appointment>)((OkObjectResult)getResult.Result).Value;
            return resData;
        }
        [Fact]
        public void Create_Appointment_Returns_Status_Created()
        {
            //Arrange
            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
                AppointmentAttachment = new Attachment()
                {
                    Content = MockDatas.base64,
                    ContentName = "Calendar",
                    ContentType = "image",
                }
            };
            AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = DateTime.Now.AddHours(6),
                EndTime = DateTime.Now.AddHours(7),
                AppointmentAttachment = new Attachment()
                {
                    Content = MockDatas.mockBase64,
                    ContentName = "Calendar",
                    ContentType = "image",
                }
            };
            //Act
            var result = _appointment.CreateAppointment(testData);
            var result_2 = _appointment.CreateAppointment(data_2);
            var result_3 = _appointment.CreateAppointment(data_3);
            var postResult = result as CreatedResult;
            var postResult_2 = result_2 as CreatedResult;
            var postResult_3 = result_3 as CreatedResult;
            var getResult = _appointment.GetAppointmentsByDate(testData.Date);
            var resultData = GetTestData(getResult);
            var res = getResult.Result as OkObjectResult;
            var testId = resultData[0].Id;

            //Assert
            Assert.Equal("test", resultData[0].Title);
            Assert.Equal(testData.StartTime, resultData[0].StartTime);
            Assert.Equal(testData.EndTime, resultData[0].EndTime);
            Assert.Equal("Reminder", resultData[0].Type);
            Assert.Equal(201, postResult?.StatusCode);
            Assert.Equal(201, postResult_2?.StatusCode);
            Assert.Equal(201, postResult_2?.StatusCode);
            Assert.Equal(200, res?.StatusCode);
            Assert.True(postResult?.Value?.Equals(true));

            Assert.IsType<List<Appointment>>(resultData);

            var removeResult = _appointment.Remove(testId, resultData[0].Date) as NoContentResult;
            _appointment.Remove(resultData[0].Id, resultData[0].Date);
            _appointment.Remove(resultData[0].Id, resultData[0].Date);
            Assert.Equal(204, removeResult?.StatusCode);
        }
        [Fact]
        public void Create_Appoinments_Are_Sorted()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test-2",
                Description = "duplicate-test",
                Type = "Event",
                StartTime = DateTime.Now.AddMinutes(4),
                EndTime = DateTime.Now.AddMinutes(25),
                AppointmentAttachment = new Attachment()
                {
                    Content = MockDatas.base64,
                    ContentName = "Calendar",
                    ContentType = "image",
                }
            };
            //Act
            var result = _appointment.CreateAppointment(testData);
            var result_2 = _appointment.CreateAppointment(data);
            var getResult = _appointment.GetAppointmentsByDate(data.Date);
            var resData = GetTestData(getResult);

            //Assert
            Assert.Equal("Event", resData[0].Type);
            Assert.Equal(testData.Type, resData[1].Type);
            Assert.Equal(data.StartTime, resData[0].StartTime);
            Assert.Equal(data.EndTime, resData[0].EndTime);
            Assert.Equal(testData.StartTime, resData[1].StartTime);
            Assert.Equal(testData.EndTime, resData[1].EndTime);

            _appointment.Remove(resData[0].Id, resData[0].Date);
            _appointment.Remove(resData[0].Id, resData[0].Date);

        }
        [Fact]
        public void Create_Appointment_Returns_Status_Conflict_When_Meeting_Is_Already_Assigned()
        {
            //Arrange

            //--- both start and end time conflicts;

            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test-2",
                Description = "test-case",
                Type = "Reminder",
                StartTime = DateTime.Now.AddMinutes(35),
                EndTime = DateTime.Now.AddMinutes(50),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            //--- new starttime is between other meetings

            AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test-3",
                Description = "test-case",
                Type = "Event",
                StartTime = DateTime.Now.AddMinutes(45),
                EndTime = DateTime.Now.AddHours(6),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            //---end-time conflicts with other meetings
            AddNewAppointment data_4 = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test-3",
                Description = "test-case",
                Type = "Event",
                StartTime = DateTime.Now.AddMinutes(20),
                EndTime = DateTime.Now.AddMinutes(45),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            //---other meeting is within new time
            AddNewAppointment data_5 = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test-3",
                Description = "test-case",
                Type = "Event",
                StartTime = DateTime.Now.AddMinutes(20),
                EndTime = DateTime.Now.AddHours(7),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            //Act
            var result = _appointment.CreateAppointment(testData);
            var result_2 = _appointment.CreateAppointment(data_2);

            var result_3 = _appointment.CreateAppointment(data_3);
            var result_4 = _appointment.CreateAppointment(data_4);
            var result_5 = _appointment.CreateAppointment(data_5);

            var postResult = result_2 as ConflictObjectResult;
            var postResult_2 = result_3 as ConflictObjectResult;
            var postResult_3 = result_4 as ConflictObjectResult;
            var postResult_4 = result_5 as ConflictObjectResult;


            var getResult = _appointment.GetAppointmentsByDate(testData.Date);
            var getData = GetTestData(getResult);

            //Assert
            // Assert.Equal(409, postResult?.StatusCode);
            Assert.Equal(409, postResult_2?.StatusCode);
            Assert.Equal(409, postResult_3?.StatusCode);
            Assert.Equal(409, postResult_4?.StatusCode);

            var removeResult = _appointment.Remove(getData[0].Id, testData.Date) as NoContentResult;
            Assert.Equal(204, removeResult.StatusCode);
        }

        [Fact]
        public void Create_Appointment_Returns_Status_Badrequest_Time_Is_Same()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 31),
                Title = "test-3",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 11, 0, 0),
                EndTime = new DateTime(2023, 01, 31, 11, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            //Act
            var result = _appointment.CreateAppointment(data) as BadRequestObjectResult;
            //Assert
            var value = result?.Value;
            var value_1 = JsonConvert.DeserializeObject<CustomCodes>((string)value);
            Assert.Equal("Cannot Add Event As StartTime And End Time Are Same", value_1.message);
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public void Create_Appointment_Returns_Status_Badrequest_When_EndTime_Lesser_Than_StartTime()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = DateTime.Now,
                Title = "test-3",
                Description = "test-case",
                Type = "Reminder",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(-30),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            //Act
            var result = _appointment.CreateAppointment(data) as BadRequestObjectResult;
            var value = JsonConvert.DeserializeObject<CustomCodes>((string)result.Value);
            //Assert
            Assert.Equal("Cannot initiate event as StartTime is Greater than endTime", value.message);
            Assert.Equal(400, result?.StatusCode);
        }

        [Fact]
        public void Create_Appointment_Returns_Status_Badrequest_StartTime_Is_Past_Time_And_Base64_Format_Is_Wrong()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = DateTime.Now,
                Title = "test-5",
                Description = "test-case",
                Type = "Reminder",
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now,
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = DateTime.Now,
                Title = "test-5",
                Description = "test-case",
                Type = "Reminder",
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(3),
                AppointmentAttachment = new Attachment()
                {
                    Content = "ljlkhgvhhcvhnmjguychcvmghj",
                    ContentName = "Calender",
                    ContentType = "image",
                }
            };

            AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = DateTime.Now,
                Title = "test-5",
                Description = "test-case",
                Type = "Reminder",
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(3),
                AppointmentAttachment = new Attachment()
                {
                    Content = "data:image/webp;base64,rdrytfyugukhijlolityd",
                    ContentName = "Calender",
                    ContentType = "image",
                }
            };

            AddNewAppointment data_4 = new AddNewAppointment()
            {
                Date = DateTime.Now,
                Title = "test-5",
                Description = "test-case",
                Type = "Reminder",
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(3),
                AppointmentAttachment = new Attachment()
                {
                    Content = MockDatas.base64,
                    ContentName = " ",
                    ContentType = "image",
                }
            };

            //Act
            var result = _appointment.CreateAppointment(data) as BadRequestObjectResult;
            var result_2 = _appointment.CreateAppointment(data_2) as BadRequestObjectResult;
            var result_3 = _appointment.CreateAppointment(data_3) as BadRequestObjectResult;
            var result_4 = _appointment.CreateAppointment(data_4) as BadRequestObjectResult;
            var value = JsonConvert.DeserializeObject<CustomCodes>((string)result.Value);
            var value_2 = JsonConvert.DeserializeObject<CustomCodes>((string)result_2.Value);

            //Assert
            Assert.Equal("Unable to add Event in current event-duration", value?.message);
            Assert.Equal("Invalid Content as Attachment", value_2?.message);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(400, result_2.StatusCode);
            Assert.Equal(400, result_3?.StatusCode);
            Assert.Equal(400, result_4?.StatusCode);
        }

        [Fact]
        public void Get_Appointments__Returns_Status_Ok()
        {
            //Act
            var create = _appointment.CreateAppointment(testData);
            var result = _appointment.GetAppointmentsByDate(testData.Date);
            var getResult = result.Result as OkObjectResult;
            var getData = GetTestData(result);
            //Assert

            Assert.Equal(200, getResult?.StatusCode);
            Assert.IsType<List<Appointment>>(getResult?.Value);
            Assert.Equal("test", getData[0].Title);
            Assert.Equal("Reminder", getData[0].Type);

            _appointment.Remove(getData[0].Id, getData[0].Date);
        }

        [Fact]
        public void Get_List_By_Range_Returns_Ok_And_Bad_Request()
        {
            //Arrange
            AddNewAppointment data_1 = new AddNewAppointment()
            {
                Date = new DateTime(2024, 01, 26),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2024, 01, 26, 11, 0, 0),
                EndTime = new DateTime(2024, 01, 26, 12, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = new DateTime(2024, 01, 27),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2024, 01, 27, 11, 0, 0),
                EndTime = new DateTime(2024, 01, 27, 12, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = new DateTime(2024, 01, 27),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2024, 01, 27, 13, 0, 0),
                EndTime = new DateTime(2024, 01, 27, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            AddNewAppointment data_4 = new AddNewAppointment()
            {
                Date = new DateTime(2024, 01, 28),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2024, 01, 28, 13, 0, 0),
                EndTime = new DateTime(2024, 01, 28, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            AddNewAppointment data_5 = new AddNewAppointment()
            {
                Date = new DateTime(2024, 01, 29),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2024, 01, 29, 13, 0, 0),
                EndTime = new DateTime(2024, 01, 29, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            AddNewAppointment data_6 = new AddNewAppointment()
            {
                Date = new DateTime(2024, 01, 30),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2024, 01, 30, 13, 0, 0),
                EndTime = new DateTime(2024, 01, 30, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            //--out of range event
            AddNewAppointment data_7 = new AddNewAppointment()
            {
                Date = new DateTime(2024, 02, 03),
                Title = "test",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2024, 02, 03, 13, 0, 0),
                EndTime = new DateTime(2024, 02, 03, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            //get-ignored
            AddNewAppointment data_8 = new AddNewAppointment()
            {
                Date = new DateTime(2024, 01, 25),
                Title = "test-8",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2024, 01, 25, 13, 0, 0),
                EndTime = new DateTime(2024, 01, 25, 14, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            DateTime startRange = new DateTime(2024, 01, 26);
            DateTime endRange = startRange.AddDays(7);
            DateTime endRange_2 = startRange.AddDays(-1);
            //Act
            _appointment.CreateAppointment(data_1);
            _appointment.CreateAppointment(data_2);
            _appointment.CreateAppointment(data_3);
            _appointment.CreateAppointment(data_4);
            _appointment.CreateAppointment(data_5);
            _appointment.CreateAppointment(data_6);
            _appointment.CreateAppointment(data_7);
            _appointment.CreateAppointment(data_8);

            var getResult = _appointment.GetListByRange(startRange, endRange);
            var getResult_2 = _appointment.GetListByRange(startRange, endRange_2);
            var value = getResult.Result as OkObjectResult;
            var value_2 = getResult_2.Result as BadRequestObjectResult;
            var getData = GetTestData(getResult);
            var getResultData_7 = _appointment.GetAppointmentsByDate(data_7.Date);
            var getResultData_8 = _appointment.GetAppointmentsByDate(data_8.Date);
            var getData_2 = GetTestData(getResultData_7);
            var getData_3 = GetTestData(getResultData_8);

            //Assert
            Assert.Equal(200, value?.StatusCode);
            Assert.Equal(6, getData.Count); //---out of range element not added;
            Assert.InRange(getData[0].EndTime, startRange, endRange);
            Assert.InRange(getData[5].EndTime, startRange, endRange);
            Assert.Equal(400, value_2.StatusCode);

            var remove_1 = _appointment.Remove(getData[5].Id, getData[5].Date);
            var remove_2 = _appointment.Remove(getData[4].Id, getData[4].Date);
            var remove_3 = _appointment.Remove(getData[3].Id, getData[3].Date);
            var remove_4 = _appointment.Remove(getData[2].Id, getData[2].Date);
            var remove_5 = _appointment.Remove(getData[1].Id, getData[1].Date);
            var remove_6 = _appointment.Remove(getData[0].Id, getData[0].Date);
            _appointment.Remove(getData_2[0].Id, getData_2[0].Date);
            _appointment.Remove(getData_3[0].Id, getData_3[0].Date);

            // DataStore.dictionaryData.Clear();

        }

        [Fact]
        public void Get_Appointments__Returns_Empty_List()
        {
            //Arrange
            DateTime date = new DateTime(2023, 01, 15);

            //Act
            var getResult = _appointment.GetAppointmentsByDate(date);
            var result = getResult.Result as OkObjectResult;
            var getData = GetTestData(getResult);

            //Assert
            Assert.True(getData.Count == 0);
            Assert.IsType<List<Appointment>>(result?.Value);
            Assert.Empty(getData);

        }
        [Fact]
        public void Remove_Appoitments()
        {
            //Arrange

            //Act
            var create = _appointment.CreateAppointment(testData);
            var getResult = _appointment.GetAppointmentsByDate(testData.Date);
            var getData = GetTestData(getResult);
            var result = _appointment.Remove(getData[0].Id, getData[0].Date) as NoContentResult;
            //Assert
            Assert.Equal(204, result?.StatusCode);
        }
        [Fact]
        public void Remove_Appoitments_Returns_Status_NotFound_Wrong_Id_Wrong_Date()
        {
            //Arrange
            var createResult = _appointment.CreateAppointment(testData);
            DateTime date = new DateTime(2023, 01, 13, 0, 0, 0);
            //Act
            //--id-invalid
            var result = _appointment.Remove(new Guid("eaa24756-3fac-4e46-b4bb-074ff4f6b864"), testData.Date);
            var removeResult = result as NotFoundObjectResult;
            var getResult = _appointment.GetAppointmentsByDate(testData.Date);
            var getData = GetTestData(getResult);

            //--date-not-matched

            var result_2 = _appointment.Remove(getData[0].Id, date);
            var removeResult_2 = result_2 as NotFoundObjectResult;

            //Assert
            Assert.Equal(404, removeResult?.StatusCode);
            Assert.Equal(404, removeResult_2?.StatusCode);

            _appointment.Remove(getData[0].Id, testData.Date);

        }
        [Fact]
        public void Update_Appoitments_Returns_Status_Ok()
        {
            //Arrange

            //Act
            var createResult = _appointment.CreateAppointment(testData);
            var getResult = _appointment.GetAppointmentsByDate(testData.Date);
            var getData = GetTestData(getResult);

            var getId = getData[0].Id;
            var toUpdate = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getId,
                    Date = testData.Date,
                    Title = "test-updated",
                    Description = "test-case",
                    Type = "out of office",
                    StartTime = DateTime.Now.AddHours(3),
                    EndTime = DateTime.Now.AddHours(5),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = testData.Date
            });
            var updatedResult = (OkObjectResult)toUpdate;
            var getUpdatedResult = _appointment.GetAppointmentsByDate(testData.Date);
            var result = GetTestData(getUpdatedResult);

            //update appointment to another day
            UpdateAppointment update_data = new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getId,
                    Date = new DateTime(2023, 02, 01),
                    Title = "test-updated-to-another-date",
                    Description = "test-case",
                    Type = "Event",
                    StartTime = new DateTime(2023, 02, 01, 17, 0, 0),
                    EndTime = new DateTime(2023, 02, 01, 17, 30, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = MockDatas.base64,
                        ContentName = "Calendar",
                        ContentType = "image",
                    },
                },
                OldDate = testData.Date
            };

            //Assert
            Assert.Equal(200, updatedResult?.StatusCode);
            Assert.Equal("test-updated", result[0].Title);
            Assert.Equal("test-case", result[0].Description);
            Assert.Equal("out of office", result[0].Type);

            var updateEvent = _appointment.Update(update_data);
            var getUpdatedResult_2 = _appointment.GetAppointmentsByDate(update_data.Appointment.Date);
            var result_2 = GetTestData(getUpdatedResult_2);

            Assert.Equal(update_data.Appointment.Type, result_2[0].Type);
            Assert.Equal(update_data.Appointment.Title, result_2[0].Title);


            _appointment.Remove(result_2[0].Id, result_2[0].Date);

        }
        [Fact]
        public void Update_Appoitments_Returns_Status_Conflict_Meeting_Is_Already_Assigned()
        {
            //Arrange
            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test-4",
                Description = "test-case-scenario",
                Type = "Event",
                StartTime = DateTime.Now.AddHours(4.5),
                EndTime = DateTime.Now.AddHours(5),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            AddNewAppointment data_3 = new AddNewAppointment()
            {
                Date = new DateTime(2023, 02, 02),
                Title = "test-4",
                Description = "test-case-scenario",
                Type = "Event",
                StartTime = new DateTime(2023, 02, 02, 2, 0, 0),
                EndTime = new DateTime(2023, 02, 02, 3, 0, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            //Act
            var newEvent = _appointment.CreateAppointment(testData);
            var createEvent = _appointment.CreateAppointment(data_2);
            _appointment.CreateAppointment(data_3);
            var getResult1 = _appointment.GetAppointmentsByDate(testData.Date);
            var getResult = _appointment.GetAppointmentsByDate(data_2.Date);
            var getResult_2 = _appointment.GetAppointmentsByDate(data_3.Date);
            var getData = GetTestData(getResult);
            var getData_2 = GetTestData(getResult_2);

            //starttime and endtime conflicts

            var updateEvent = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[1].Id,
                    Date = data_2.Date,
                    Title = "test-4",
                    Description = "test-case-scenario",
                    Type = "Event",
                    StartTime = DateTime.Now.AddMinutes(32),
                    EndTime = DateTime.Now.AddHours(1),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date,
            });

            //starttime conflicts
            var updateEvent_2 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[1].Id,
                    Date = data_2.Date,
                    Title = "test-4",
                    Description = "test-case-scenario",
                    Type = "Event",
                    StartTime = DateTime.Now.AddMinutes(45),
                    EndTime = DateTime.Now.AddHours(6),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });
            //endtime conflict

            var updateEvent_3 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[1].Id,
                    Date = data_2.Date,
                    Title = "test-4",
                    Description = "test-case-scenario",
                    Type = "Event",
                    StartTime = DateTime.Now.AddMinutes(5),
                    EndTime = DateTime.Now.AddMinutes(50),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });

            //other meeting is within new meettime

            var updateEvent_4 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[1].Id,
                    Date = data_2.Date,
                    Title = "test-4",
                    Description = "test-case-scenario",
                    Type = "Event",
                    StartTime = DateTime.Now.AddMinutes(5),
                    EndTime = DateTime.Now.AddHours(7),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });

            //conflict with other day meeting
            var updateEvent_5 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[1].Id,
                    Date = data_3.Date,
                    Title = "test-new-date",
                    Description = "test-case-scenario",
                    Type = "Reminder",
                    StartTime = new DateTime(2023, 02, 02, 2, 0, 0),
                    EndTime = new DateTime(2023, 02, 02, 3, 0, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });

            //other meet is within this time
            var updateEvent_6 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[1].Id,
                    Date = data_3.Date,
                    Title = "test-new-date",
                    Description = "test-case-scenario",
                    Type = "Reminder",
                    StartTime = new DateTime(2023, 02, 02, 1, 45, 0),
                    EndTime = new DateTime(2023, 02, 02, 3, 45, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });

            // starttime within the other meet
            var updateEvent_7 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[1].Id,
                    Date = data_3.Date,
                    Title = "test-new-date",
                    Description = "test-case-scenario",
                    Type = "Reminder",
                    StartTime = new DateTime(2023, 02, 02, 2, 15, 0),
                    EndTime = new DateTime(2023, 02, 02, 3, 45, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });

            //end-time within the other meet
            var updateEvent_8 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[1].Id,
                    Date = data_3.Date,
                    Title = "test-new-date",
                    Description = "test-case-scenario",
                    Type = "Reminder",
                    StartTime = new DateTime(2023, 02, 02, 1, 15, 0),
                    EndTime = new DateTime(2023, 02, 02, 2, 45, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });

            //Assert
            // Assert.Equal(409, updatedResult?.StatusCode);

            var result = Assert.IsType<ConflictObjectResult>(updateEvent_2);
            var result_2 = Assert.IsType<ConflictObjectResult>(updateEvent_3);
            var result_3 = Assert.IsType<ConflictObjectResult>(updateEvent_4);
            var result_4 = Assert.IsType<ConflictObjectResult>(updateEvent_5);
            var result_5 = Assert.IsType<ConflictObjectResult>(updateEvent_6);
            var result_6 = Assert.IsType<ConflictObjectResult>(updateEvent_7);
            var value = JsonConvert.DeserializeObject<CustomCodes>((string)result.Value);
            Assert.Equal(409, result_6.StatusCode);
            Assert.Equal(409, result.StatusCode);
            Assert.Equal(409, result_2.StatusCode);
            Assert.Equal(409, result_3.StatusCode);
            Assert.Equal(409, result_4.StatusCode);
            Assert.Equal(409, result_5.StatusCode);
            Assert.Equal("Meeting is already assigned", value?.message);
            //clean-up
            _appointment.Remove(getData[0].Id, getData[0].Date);
            _appointment.Remove(getData[0].Id, getData[0].Date);
            _appointment.Remove(getData_2[0].Id, getData_2[0].Date);
        }

        [Fact]
        public void Update_Appoitments_Returns_Status_Badrequest_When_Start_Time_Greater()
        {
            //Arrange
            AddNewAppointment data_2 = new AddNewAppointment()
            {
                Date = DateTime.Now,
                Title = "test-4",
                Description = "test-case-scenario",
                Type = "Event",
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };
            //Act
            _appointment.CreateAppointment(testData);
            var createEvent = _appointment.CreateAppointment(data_2);
            var getResult_1 = _appointment.GetAppointmentsByDate(testData.Date);
            var getData_1 = GetTestData(getResult_1);
            var getResult = _appointment.GetAppointmentsByDate(data_2.Date);
            var getData = GetTestData(getResult);
            var updateEvent = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[0].Id,
                    Date = data_2.Date,
                    Title = "test-4",
                    Description = "test-case-scenario",
                    Type = "Event",
                    StartTime = testData.StartTime.AddHours(4),
                    EndTime = DateTime.Now.AddMinutes(-30),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });
            var updatedResult = (BadRequestObjectResult)updateEvent;

            var value = JsonConvert.DeserializeObject<CustomCodes>((string)updatedResult.Value);
            //event duration is in past
            var updateEvent_2 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getData[0].Id,
                    Date = data_2.Date,
                    Title = "test-updated",
                    Description = "test-case",
                    Type = "out of office",
                    StartTime = DateTime.Now.AddMinutes(-30),
                    EndTime = DateTime.Now.AddMinutes(-10),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data_2.Date
            });
            var updatedResult_2 = updateEvent_2 as BadRequestObjectResult;

            var value_2 = JsonConvert.DeserializeObject<CustomCodes>((string)updatedResult_2.Value);

            //Assert
            Assert.Equal("Cannot initiate event as StartTime is Greater than endTime", value?.message);
            Assert.Equal("Unable to add Event in current event-duration", value_2?.message);
            Assert.Equal(400, updatedResult.StatusCode);
            Assert.Equal(400, updatedResult_2.StatusCode);

            _appointment.Remove(getData[0].Id, getData[0].Date);
            var removeResult = _appointment.Remove(getData_1[0].Id, getData_1[0].Date);

        }
        [Fact]
        public void Update_Appoitments_Returns_Badrequest_When_start_Time_And_End_Time_Same_And_Attachment_Invalid()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = new DateTime(2023, 01, 31, 0, 0, 0),
                Title = "test-3",
                Description = "test-case",
                Type = "Reminder",
                StartTime = new DateTime(2023, 01, 31, 4, 50, 0),
                EndTime = new DateTime(2023, 01, 31, 5, 20, 0),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            //Act
            var createResult = _appointment.CreateAppointment(data);
            var getResult = _appointment.GetAppointmentsByDate(data.Date);
            var getData = GetTestData(getResult);

            var getId = getData[0].Id;
            var updateEvent = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getId,
                    Date = data.Date,
                    Title = "test-updated",
                    Description = "test-case",
                    Type = "out of office",
                    StartTime = new DateTime(2023, 01, 31, 4, 50, 0),
                    EndTime = new DateTime(2023, 01, 31, 4, 50, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = data.Date
            });

            var updateEvent_2 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getId,
                    Date = data.Date,
                    Title = "test-updated",
                    Description = "test-case",
                    Type = "out of office",
                    StartTime = new DateTime(2023, 01, 31, 4, 50, 0),
                    EndTime = new DateTime(2023, 01, 31, 5, 50, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = MockDatas.dummyBase64,
                        ContentName = "Calender",
                        ContentType = "image",
                    }
                },
                OldDate = data.Date
            });
            var updateEvent_3 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getId,
                    Date = data.Date,
                    Title = "test-updated",
                    Description = "test-case",
                    Type = "out of office",
                    StartTime = new DateTime(2023, 01, 31, 4, 50, 0),
                    EndTime = new DateTime(2023, 01, 31, 5, 50, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = MockDatas.base64,
                        ContentName = " ",
                        ContentType = "image",
                    }
                },
                OldDate = data.Date
            });
            var updatedResult = updateEvent as BadRequestObjectResult;
            var updatedResult_2 = updateEvent_2 as BadRequestObjectResult;
            var updatedResult_3 = updateEvent_3 as BadRequestObjectResult;

            var value = JsonConvert.DeserializeObject<CustomCodes>((string)updatedResult.Value);
            var value_2 = JsonConvert.DeserializeObject<CustomCodes>((string)updatedResult_2.Value);
            //Assert
            Assert.Equal(400, updatedResult.StatusCode);
            Assert.Equal(400, updatedResult_2.StatusCode);
            Assert.Equal(400, updatedResult_3.StatusCode);
            Assert.Equal("Cannot Add Event As StartTime And End Time Are Same", value?.message);
            Assert.Equal("Invalid Content as Attachment", value_2?.message);
            var removeResult = _appointment.Remove(getId, getData[0].Date);

        }

        [Fact]
        public void Update_Appoitments_Returns_Not_Found_When_Date_OR_Id_Is_Not_Correct()
        {
            //Arrange

            //Act
            var createResult = _appointment.CreateAppointment(testData);
            var getResult = _appointment.GetAppointmentsByDate(testData.Date);
            var getData = GetTestData(getResult);
            var getId = getData[0].Id;

            //---id invalid
            var updateEvent = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = new Guid("eaa24756-3fac-4e46-b4bb-074ff4f6b864"),
                    Date = testData.Date,
                    Title = "test-updated",
                    Description = "test-case",
                    Type = "out of office",
                    StartTime = DateTime.Now.AddMinutes(30),
                    EndTime = DateTime.Now.AddHours(3),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = testData.Date
            });
            var updatedResult_2 = updateEvent as NotFoundObjectResult;
            var value_2 = JsonConvert.DeserializeObject<CustomCodes>((string)updatedResult_2.Value);

            //-id-invalid
            var updateEvent_2 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = new Guid("eaa24756-3fac-4e46-b4bb-074ff4f6b864"),
                    Date = new DateTime(2023, 01, 31, 0, 0, 0),
                    Title = "test-updated",
                    Description = "test-case",
                    Type = "out of office",
                    StartTime = new DateTime(2023, 01, 31, 4, 50, 0),
                    EndTime = new DateTime(2023, 01, 31, 5, 50, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = testData.Date
            });
            //OldDate invalid
            var updateEvent_3 = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = getId,
                    Date = new DateTime(2023, 01, 31, 0, 0, 0),
                    Title = "test-updated",
                    Description = "test-case",
                    Type = "out of office",
                    StartTime = new DateTime(2023, 01, 31, 4, 50, 0),
                    EndTime = new DateTime(2023, 01, 31, 5, 50, 0),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = new DateTime(2023, 01, 31)
            });
            var updateResult_3 = (NotFoundObjectResult)(updateEvent_3);
            var value_3 = JsonConvert.DeserializeObject<CustomCodes>((string)updateResult_3.Value);
            var updateResult = Assert.IsType<NotFoundObjectResult>(updateEvent_2);
            //Assert

            Assert.Equal("Enter Valid Id", value_2?.message);
            Assert.Equal("Invalid Date", value_3?.message);
            Assert.Equal(404, updateResult.StatusCode);
            Assert.Equal(404, updatedResult_2.StatusCode);
            Assert.Equal(404, updateResult_3.StatusCode);
            var removeResult = _appointment.Remove(getId, getData[0].Date);
        }

        [Fact]
        public void Update_Appoinments_Are_Sorted()
        {
            //Arrange
            AddNewAppointment data = new AddNewAppointment()
            {
                Date = testData.Date,
                Title = "test-2",
                Description = "duplicate-test",
                Type = "Event",
                StartTime = DateTime.Now.AddMinutes(4),
                EndTime = DateTime.Now.AddMinutes(25),
                AppointmentAttachment = new Attachment()
                {
                    Content = "",
                    ContentName = "",
                    ContentType = "",
                }
            };

            //Act
            var result = _appointment.CreateAppointment(testData);
            var result_2 = _appointment.CreateAppointment(data);
            var getResult = _appointment.GetAppointmentsByDate(data.Date);
            var resData = GetTestData(getResult);

            Assert.Equal("Event", resData[0].Type);
            Assert.Equal(testData.Type, resData[1].Type);

            var updateEvent = _appointment.Update(new UpdateAppointment()
            {
                Appointment = new Appointment()
                {
                    Id = resData[0].Id,
                    Date = testData.Date,
                    Title = "test-2",
                    Description = "duplicate-test-updated",
                    Type = "Event",
                    StartTime = DateTime.Now.AddHours(4),
                    EndTime = DateTime.Now.AddHours(6),
                    AppointmentAttachment = new Attachment()
                    {
                        Content = "",
                        ContentName = "",
                        ContentType = "",
                    }
                },
                OldDate = testData.Date
            });
            var getResult_2 = _appointment.GetAppointmentsByDate(data.Date);
            var resData_2 = GetTestData(getResult_2);
            //Assert
            Assert.Equal(testData.Title, resData_2[0].Title);
            Assert.Equal("test-2", resData_2[1].Title);
            Assert.Equal("Event", resData_2[1].Type);
            Assert.Equal(testData.StartTime, resData[0].StartTime);
            Assert.Equal(testData.EndTime, resData[0].EndTime);

            _appointment.Remove(resData[0].Id, resData[0].Date);
            _appointment.Remove(resData[0].Id, resData[0].Date);


        }


    }
}
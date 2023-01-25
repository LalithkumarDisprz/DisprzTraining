using DisprzTraining.Models.CustomCodeModel;

namespace DisprzTraining.CustomErrorCodes
{
    public static class CustomErrorCodeMessages
    {
        public static CustomCodes startTimeGreaterThanEndTime = new CustomCodes()
        {
            message = "Cannot initiate event as StartTime is Greater than endTime",
            errorCode = "START_TIME_GREATER_THAN_END_TIME",
        };
        public static CustomCodes startAndEndTimeAreSame = new CustomCodes()
        {
            message = "Cannot Add Event As StartTime And End Time Are Same",
            errorCode = "START_TIME_AND_END_TIME_ARE_THE_SAME",
        };
        public static CustomCodes tryingToAddMeetingInPastDate = new CustomCodes()
        {
            message = "Unable to add Event in current event-duration",
            errorCode = "START_TIME_IS_LESS_THAN_CURRENT_TIME",
        };
        public static CustomCodes meetingIsAlreadyAssigned = new CustomCodes()
        {
            message = "Meeting is already assigned",
            errorCode = "MEETING_IS_ALREADY-ASSIGNED",
        };
        public static CustomCodes idIsInvalid = new CustomCodes()
        {
            message = "Enter Valid Id",
            errorCode = "Id_Is_InValid",
        };
        public static CustomCodes invalidDate = new CustomCodes()
        {
            message = "Invalid Date",
            errorCode = "INVALID_DATE",
        };
        public static CustomCodes invalidDateOrID = new CustomCodes()
        {
            message = "Invalid Date or id",
            errorCode = "INVALID_DATE_ID",
        };
        
        public static CustomCodes invalidAttachment = new CustomCodes()
        {
            message = "Invalid Content as Attachment",
            errorCode = "INVALID_ATTACHMENT",
        };
        public static CustomCodes invalidInputs = new CustomCodes()
        {
            message = "Unable to process the data check your inputs",
            errorCode = "INVALID_INPUTS",
        };
    }
}
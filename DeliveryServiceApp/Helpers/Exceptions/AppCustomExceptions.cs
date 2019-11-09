using System.Net;

namespace DeliveryServiceApp.Helpers.Exceptions
{
    public static class AppCustomExceptions
    {
        public static readonly AppCustomException UsernameInvalidValue = new AppCustomException(1, "Username cannot be null, empty or whitespace.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException PasswordInvalidValue = new AppCustomException(2, "Password cannot be null, empty or whitespace.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException UsernameAlreadyInUse = new AppCustomException(3, "The given username is already in use.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException UserNotFound = new AppCustomException(4, "The specified user was not found.", (int)HttpStatusCode.BadRequest);
        public static readonly AppCustomException RoleInvalidValue = new AppCustomException(5, "The defined user's role is not valid.", (int)HttpStatusCode.BadRequest);
        public static readonly AppCustomException PointInvalidValue = new AppCustomException(6, "Point's description cannot be null, empty or whitespace.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException PointDescriptionAlreadyInUse = new AppCustomException(7, "The given point's description is already in use.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException PointNotFound = new AppCustomException(8, "Specified point was not found.", (int)HttpStatusCode.BadRequest);
        public static readonly AppCustomException RouteBaseAlreadyExists = new AppCustomException(9, "A route with the same origin and destination already exists.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException EqualPointsInRouteStep = new AppCustomException(10, "The origin and destination points of a route or step cannot be the same.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException RouteBaseNotFound = new AppCustomException(11, "Specified route base was not found.", (int)HttpStatusCode.BadRequest);
        public static readonly AppCustomException StepAlreadyExists = new AppCustomException(12, "A step with the same start and end points already exists.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException StepNotFound = new AppCustomException(13, "Specified step was not found.", (int)HttpStatusCode.BadRequest);
        public static readonly AppCustomException StepTimeCostPositive = new AppCustomException(14, "The decimals time and cost of a step must be positive.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException RouteBaseNotExists = new AppCustomException(15, "A route with the specified origin and destination does not exist.", (int)HttpStatusCode.UnprocessableEntity);
        public static readonly AppCustomException StepBeginRouteNotFound = new AppCustomException(16, "No step corresponding to the beginning of the route was found.", (int)HttpStatusCode.BadRequest);
        public static readonly AppCustomException StepEndRouteNotFound = new AppCustomException(17, "No step corresponding to the destination of the route was found.", (int)HttpStatusCode.BadRequest);

    }
}

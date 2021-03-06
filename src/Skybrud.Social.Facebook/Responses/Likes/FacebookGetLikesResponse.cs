using Skybrud.Social.Facebook.Models.Likes;
using Skybrud.Social.Http;

namespace Skybrud.Social.Facebook.Responses.Likes {

    /// <summary>
    /// Class representing a response of a request to get a collection of <see cref="FacebookLike"/> of a given
    /// Facebook object.
    /// </summary>
    public class FacebookGetLikesResponse : FacebookResponse<FacebookLikesCollection> {

        #region Constructors

        private FacebookGetLikesResponse(SocialHttpResponse response) : base(response) {

            // Validate the response
            ValidateResponse(response);

            // Parse the response body
            Body = ParseJsonObject(response.Body, FacebookLikesCollection.Parse);

        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses the specified <paramref name="response"/> into an instance of <see cref="FacebookGetLikesResponse"/>.
        /// </summary>
        /// <param name="response">The instance of <see cref="SocialHttpResponse"/> representing the raw response.</param>
        /// <returns>An instance of <see cref="FacebookGetLikesResponse"/> representing the response.</returns>
        public static FacebookGetLikesResponse ParseResponse(SocialHttpResponse response) {
            return response == null ? null : new FacebookGetLikesResponse(response);
        }

        #endregion

    }

}
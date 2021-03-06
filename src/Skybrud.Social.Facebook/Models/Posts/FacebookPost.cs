using System;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Time;
using Skybrud.Social.Facebook.Constants;
using Skybrud.Social.Facebook.Models.Applications;
using Skybrud.Social.Facebook.Models.Comments;
using Skybrud.Social.Facebook.Models.Common;
using Skybrud.Social.Facebook.Models.Common.Tags;
using Skybrud.Social.Facebook.Models.Likes;
using Skybrud.Social.Facebook.Models.Places;
using Skybrud.Social.Interfaces;

namespace Skybrud.Social.Facebook.Models.Posts {

    /// <summary>
    /// Class representing a Facebook post.
    /// </summary>
    /// <see>
    ///     <cref>https://developers.facebook.com/docs/graph-api/reference/v2.12/post</cref>
    /// </see>
    public class FacebookPost : FacebookObject, ISocialTimelineEntry {

        #region Properties

        /// <summary>
        /// Gets the ID of the post.
        /// </summary>
        public string Id { get; private set; }
        
        /// <summary>
        /// Gets information about the app or business that created the post. Applies to pages only.
        /// </summary>
        public FacebookEntity AdminCreator { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="AdminCreator"/> property was included in the response.
        /// </summary>
        public bool HasAdminCreator {
            get { return AdminCreator != null; }
        }
        
        /// <summary>
        /// Gets information about the app the post was published by.
        /// </summary>
        public FacebookApplication Application { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Application"/> property was included in the response.
        /// </summary>
        public bool HasApplication {
            get { return Application != null; }
        }

        /// <summary>
        /// Gets a collection of the attachments of the post. Not all posts have attachments.
        /// </summary>
        public FacebookPostAttachments Attachments { get; }

        /// <summary>
        /// Gets whether the response for the post included any attachments.
        /// </summary>
        public bool HasAttachments => Attachments != null && Attachments.Count > 0;

        // TODO: Add support for the "call_to_action" property

        /// <summary>
        /// Gets the link caption in post that appears below name.
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Caption"/> property was included in the response.
        /// </summary>
        public bool HasCaption {
            get { return !String.IsNullOrWhiteSpace(Caption); }
        }

        /// <summary>
        /// Gets the time the post was initially published. For a post about a life event, this will be the date and
        /// time of the life event.
        /// </summary>
        public EssentialsDateTime CreatedTime { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="CreatedTime"/> property was included in the response.
        /// </summary>
        public bool HasCreatedTime {
            get { return CreatedTime != null; }
        }

        /// <summary>
        /// Gets a description of a link in the post (appears beneath the <see cref="Caption"/>).
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Description"/> property was included in the response.
        /// </summary>
        public bool HasDescription {
            get { return !String.IsNullOrWhiteSpace(Description); }
        }

        /// <summary>
        /// Gets information about the profile that posted the message.
        /// </summary>
        public FacebookProfile From { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="From"/> property was included in the response.
        /// </summary>
        public bool HasFrom {
            get { return From != null; }
        }

        /// <summary>
        /// Gets the URL to a full-sized version of the photo published in the post or scraped from a link in the post.
        /// If the photo's largest dimension exceeds 720 pixels, it will be resized, with the largest dimension set to
        /// 720.
        /// </summary>
        public string FullPicture { get; }

        /// <summary>
        /// Gets whether the <see cref="FullPicture"/> property was included in the response.
        /// </summary>
        public bool HasFullPicture => !String.IsNullOrWhiteSpace(FullPicture);

        /// <summary>
        /// Gets a link to an icon representing the type of this post.
        /// </summary>
        public string Icon { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Icon"/> property was included in the response.
        /// </summary>
        public bool HasIcon {
            get { return !String.IsNullOrWhiteSpace(Icon); }
        }

        // TODO: Add support for the "instagram_eligibility" property

        /// <summary>
        /// Gets whether this post is marked as hidden (Applies to Pages only).
        /// </summary>
        public bool IsHidden { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="IsHidden"/> property was included in the response.
        /// </summary>
        public bool HasIsHidden {
            get { return HasJsonProperty(FacebookPostFields.IsHidden.Name); }
        }
        
        // TODO: Add support for the "is_instagram_eligible" property

        /// <summary>
        /// Gets whether this post is marked as hidden (Applies to Pages only).
        /// </summary>
        public bool IsPublished { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="IsPublished"/> property was included in the response.
        /// </summary>
        public bool HasIsPublished {
            get { return HasJsonProperty("is_published"); }
        }

        /// <summary>
        /// Gets the link attached to this post.
        /// </summary>
        public string Link { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Link"/> property was included in the response.
        /// </summary>
        public bool HasLink {
            get { return !String.IsNullOrWhiteSpace(Link); }
        }

        /// <summary>
        /// Gets the status message in the post.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Message"/> property was included in the response.
        /// </summary>
        public bool HasMessage {
            get { return !String.IsNullOrWhiteSpace(Message); }
        }

        /// <summary>
        /// Gets an array of the profiles tagged in the <see cref="Message"/> property.
        /// </summary>
        public FacebookProfileTag[] MessageTags { get; }

        /// <summary>
        /// Gets whether the <see cref="MessageTags"/> property was included in the response.
        /// </summary>
        public bool HasMessageTags => MessageTags.Length > 0;

        /// <summary>
        /// Gets the name of the <see cref="Link"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Name"/> property was included in the response.
        /// </summary>
        public bool HasName {
            get { return !String.IsNullOrWhiteSpace(Name); }
        }
        
        /// <summary>
        /// Gets the ID of any uploaded photo or video attached to the post.
        /// </summary>
        public string ObjectId { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="ObjectId"/> property was included in the response.
        /// </summary>
        public bool HasObjectId {
            get { return !String.IsNullOrWhiteSpace(ObjectId); }
        }
        
        /// <summary>
        /// Gets the ID of a parent post for this post, if it exists. For example, if this story is a
        /// 'Your Page was mentioned in a post' story, the <see cref="ParentId"/> will be the original post where the
        /// mention happened.
        /// </summary>
        public string ParentId { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="ParentId"/> property was included in the response.
        /// </summary>
        public bool HasParentId {
            get { return !String.IsNullOrWhiteSpace(ParentId); }
        }
        
        /// <summary>
        /// Gets the URL to the permalink page of the post.
        /// </summary>
        public string PermalinkUrl { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="PermalinkUrl"/> property was included in the response.
        /// </summary>
        public bool HasPermalinkUrl {
            get { return !String.IsNullOrWhiteSpace(PermalinkUrl); }
        }

        /// <summary>
        /// Gets the picture scraped from any <see cref="Link"/> included with the post.
        /// </summary>
        public string Picture { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Picture"/> property was included in the response.
        /// </summary>
        public bool HasPicture {
            get { return !String.IsNullOrWhiteSpace(Picture); }
        }

        /// <summary>
        /// Gets any location information attached to the post.
        /// </summary>
        public FacebookPlace Place { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Place"/> property was included in the response.
        /// </summary>
        public bool HasPlace {
            get { return Place != null; }
        }

        /// <summary>
        /// Gets the privacy settings of the post.
        /// </summary>
        public FacebookPostPrivacy Privacy { get; }

        /// <summary>
        /// Gets whether the <see cref="Privacy"/> property was included in the response.
        /// </summary>
        public bool HasPrivacy => Privacy != null;

        /// <summary>
        /// Gets a list of properties for any attached video, for example, the length of the video.
        /// </summary>
        public FacebookPostProperty[] Properties { get; private set; }

        /// <summary>
        /// Gets whether the post has any <see cref="Properties"/>.
        /// </summary>
        public bool HasProperties {
            get { return Properties.Length > 0; }
        }
        
        /// <summary>
        /// Gets information about how many times the post has been shared. If the post hasn't yet
        /// been shared, this property will return <code>null</code>.
        /// </summary>
        public FacebookShares Shares { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Shares"/> property has a value.
        /// </summary>
        public bool HasShares {
            get { return Shares != null; }
        }

        /// <summary>
        /// Gets an object with information about how the entry has been liked.
        /// </summary>
        public FacebookLikesCollection Likes { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Likes"/> property has a value.
        /// </summary>
        public bool HasLikes {
            get { return Likes != null; }
        }
        
        /// <summary>
        /// Gets an object with information about how the entry has been commented.
        /// </summary>
        public FacebookCommentsCollection Comments { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Comments"/> property has a value.
        /// </summary>
        public bool HasComments {
            get { return Comments != null; }
        }
        
        /// <summary>
        /// Gets a URL to any Flash movie or video file attached to the post.
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Source"/> property was included in the response.
        /// </summary>
        public bool HasSource {
            get { return !String.IsNullOrWhiteSpace(Source); }
        }
        
        /// <summary>
        /// Gets the type of a status update.
        /// </summary>
        public FacebookPostStatusType StatusType { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="StatusType"/> property was included in the response.
        /// </summary>
        public bool HasStatusType {
            get { return StatusType == FacebookPostStatusType.NotSpecified; }
        }

        /// <summary>
        /// Gets text from stories not intentionally generated by users, such as those generated when two people become
        /// friends, or when someone else posts on the person's wall.
        /// </summary>
        public string Story { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Story"/> property was included in the response.
        /// </summary>
        public bool HasStory {
            get { return !String.IsNullOrWhiteSpace(Story); }
        }

        /// <summary>
        /// Gets an array of the profiles tagged in the <see cref="Story"/> property.
        /// </summary>
        public FacebookProfileTag[] StoryTags { get; }

        /// <summary>
        /// Gets whether the <see cref="StoryTags"/> property was included in the response.
        /// </summary>
        public bool HasStoryTags => StoryTags.Length > 0;

        // TODO: Add support for the "targeting" property
        
        // TODO: Add support for the "to" property

        /// <summary>
        /// Gets the object type of this post.
        /// </summary>
        public FacebookPostType Type { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="Type"/> property was included in the response.
        /// </summary>
        public bool HasType {
            get { return Type == FacebookPostType.NotSpecified; }
        }

        /// <summary>
        /// Gets the time when the post was created, last edited or the time of the last comment that was left on the
        /// post.
        /// 
        /// For a post about a life event, this will be the date and time of the life event.
        /// </summary>
        public EssentialsDateTime UpdatedTime { get; private set; }

        /// <summary>
        /// Gets whether the <see cref="UpdatedTime"/> property was included in the response.
        /// </summary>
        public bool HasUpdatedTime {
            get { return UpdatedTime != null; }
        }

        // TODO: Add support for the "with_tags" property

        /// <summary>
        /// Gets a sortable date for the post. This requires the <see cref="CreatedTime"/> property to be inclided in
        /// the response - otherwise <see cref="DateTime.MinValue"/> will be returned instead.
        /// </summary>
        public EssentialsDateTime SortDate {
            get { return CreatedTime; }
        }

        #endregion

        #region Constructors

        private FacebookPost(JObject obj) : base(obj) {
            Id = obj.GetString("id");
            AdminCreator = obj.GetObject("admin_creator", FacebookEntity.Parse);
            Application = obj.GetObject("application", FacebookApplication.Parse);
            Attachments = obj.GetObject("attachments", FacebookPostAttachments.Parse);
            // TODO: Add support for the "call_to_action" property
            // TODO: Add support for the "can_reply_privately" property
            Caption = obj.GetString("caption");
            CreatedTime = obj.GetString("created_time", EssentialsDateTime.Parse);
            Description = obj.GetString("description");
            // TODO: Add support for the "feed_targeting" property
            From = obj.GetObject("from", FacebookProfile.Parse);
            FullPicture = obj.GetString("full_picture");
            Icon = obj.GetString("icon");
            // TODO: Add support for the "instagram_eligibility" property
            IsHidden = obj.GetBoolean(FacebookPostFields.IsHidden.Name); 
            // TODO: Add support for the "is_instagram_eligible" property
            IsPublished = obj.GetBoolean("is_published");
            Link = obj.GetString("link");
            Message = obj.GetString("message");
            MessageTags = obj.GetArrayItems("message_tags", FacebookProfileTag.Parse);
            Name = obj.GetString("name");
            ObjectId = obj.GetString("object_id");
            ParentId = obj.GetString("parent_id");
            PermalinkUrl = obj.GetString("permalink_url");
            Picture = obj.GetString("picture");
            Place = obj.GetObject("place", FacebookPlace.Parse);
            Privacy = obj.GetObject("privacy", FacebookPostPrivacy.Parse);
            Properties = obj.GetArray("properties", FacebookPostProperty.Parse) ?? new FacebookPostProperty[0];
            Shares = obj.GetObject("shares", FacebookShares.Parse);
            Likes = obj.GetObject("likes", FacebookLikesCollection.Parse);
            Comments = obj.GetObject("comments", FacebookCommentsCollection.Parse);
            Source = obj.GetString("source");
            StatusType = obj.GetEnum("status_type", FacebookPostStatusType.NotSpecified);
            Story = obj.GetString("story");
            StoryTags = obj.GetArrayItems("story_tags", FacebookProfileTag.Parse);
            // TODO: Add support for the "targeting" property
            // TODO: Add support for the "to" property
            Type = obj.GetEnum("type", FacebookPostType.NotSpecified);
            UpdatedTime = obj.GetString("updated_time", EssentialsDateTime.Parse);
            // TODO: Add support for the "with_tags" property
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses the specified <paramref name="obj"/> into an instance of <see cref="FacebookPost"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        /// <returns>An instance of <see cref="FacebookPost"/>.</returns>
        public static FacebookPost Parse(JObject obj) {
            return obj == null ? null : new FacebookPost(obj);
        }

        #endregion

    }

}
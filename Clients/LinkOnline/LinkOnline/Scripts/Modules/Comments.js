function InitComments(parent, section) {
    var container = document.createElement("div");
    comments.id = "pnlCommentsContainer" + section;
    container.className = "CommentsContainer";

    var comments = document.createElement("div");
    comments.id = "pnlComments" + section;
    comments.className = "Comments";

    container.appendChild(comments);

    document.body.appendChild(container);

    _AjaxRequest("/Handlers/Comments.ashx", "GetComments", "Section=", function (response) {
        response = JSON.parse(response);

        var comments = document.getElementById("pnlComments" + section);

        var comment;
        var commentHeadline;
        var commentText;

        for (var i = 0; i < response.length; i++) {
            comment = document.createElement("div");
            comment.className = "Comment";

            commentHeadline = document.createElement("div");
            commentHeadline.className = "CommentHeadline";

            commentText = document.createElement("div");
            commentText.className = "CommentText";

            commentHeadline.innerHTML = response[i].DisplayUsername +
                " - " + response[i].CreationDate;
            commentText.innerHTML = response[i].Comment;

            comment.appendChild(commentHeadline);
            comment.appendChild(commentText);

            comments.appendChild(comment);
        }
    });
}
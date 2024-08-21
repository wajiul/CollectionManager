﻿$(document).ready(function () {
    var $commentText = $('#commentText');
    var $commentButton = $('#commentButton');
    var $likeButton = $('#likeButton');
    let likeCount = parseInt($('#likeCount').text());
    let isLiked = false;
    getLikeStatus();

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/reaction")
        .build();

    async function startConnection() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.error("Error starting connection: ", err);
            setTimeout(startConnection, 5000);
        }
    }

    connection.onclose(async () => {
        await startConnection();
    });
    startConnection();

    async function sendLike(like) {
        try {
            await connection.invoke("AddLike", like);
        }
        catch (err) {
            console.log('error sending like', err);
        }
    }

    async function sendComment(comment) {
        try {
            await connection.invoke("AddComment", comment);
        }
        catch (err) {
            console.log('error sending comment', err);
        }
    }

    connection.on("GetItemLikeCount", (cnt) => {
        console.log('new like', cnt);
        isLiked = true;
        $likeButton.addClass('text-info');
        if (cnt == 1) {
            $('#likeCountContainer').removeClass('d-none');
        }
        $('#likeCount').text(cnt);
    });

    connection.on("GetComment", (comment, cnt) => {
        console.log('new comment', comment);
        let c = commentUI(comment);
        $("#commentsContainer").prepend(c);

        if (cnt == 1) {
            $('#commentCountContainer').removeClass('d-none');
            $('#commentCount').text(cnt + ' comment');
        }
        else {
            $('#commentCount').text(cnt + ' comments');
        }

        $commentText.val('');
        $commentButton.hide();
    });


    if (isLiked == true) {
        console.log('likeed');
        $likeButton.addClass('text-info');
    }


    $('#comment-inputButton').on('click', function () {
        $('#comment-input').removeClass('d-none');
    });

    $('#comment-cancel').on('click', function () {
        $('#comment-input').addClass('d-none');
    });

    $commentText.on('focus', function () {
        $commentButton.show();
    });


    $commentText.on('input', function () {
        if ($commentText.val().trim() !== "") {
            $commentButton.prop('disabled', false);
        } else {
            $commentButton.prop('disabled', true);
        }
    });

    $('#commentButton').click(function () {
        var comment = {
            text: $('#commentText').val(),
            itemId: parseInt('@itemId'),
            userId: '@userId',
            createdAt: new Date()
        };

        postComment(comment);
    });

    $likeButton.click(function () {
        let like = {
            itemId: parseInt('@itemId'),
            userId: '@userId'
        };

        if (isLiked == false) {
            postLike(like);
        }
    });

    function getLikeStatus() {
        $.ajax({
            url: '/items/@itemId/isliked',
            type: 'GET',
            contentType: 'application/json',
            success: function (response) {
                if (response == true) {
                    isLiked = true;
                    $likeButton.addClass('text-info');
                }
            }
        })
    }

    function postLike(like) {
        if (!isLiked) {
            sendLike(like);
        }
    }

    function commentUI(comment) {
        const cmt = `
                        <div class="bg-white p-2">
                        <div class="d-flex flex-row user-info">
                        <img
                            class="rounded-circle"
                            src="/images/user.jpg"
                            width="35"
                            height="35"
                        />
                        <div class="d-flex flex-column justify-content-start ms-2">
                            <a class="d-block font-weight-bold comment-name">${comment.commenter}</a>
                            <span class="comment-date text-black-50">${comment.createdAt}</span>
                            <div class="mt-2">
                                <p class="comment-text">
                                    ${comment.text}
                                </p>
                            </div>
                        </div>
                        </div>
                    </div>
                `;
        return cmt;
    }
    function postComment(comment) {
        console.log(comment);
        sendComment(comment);
    }
});
$(() => {

    $("#comment-author, #comment-text").on('keyup', () => {
        let isValid = $("#comment-author").val().trim() && $("#comment-text").val().trim();
        $("#submit-comment").prop('disabled', !isValid)
    });




});

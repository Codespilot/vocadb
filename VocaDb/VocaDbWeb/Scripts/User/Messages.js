
function getMessage(messageId) {

	$.get("../../User/Message", { messageId: messageId }, function (content) {

		$("#viewMessage").html(content);

	});

}

function initPage() {

	$(".viewMessage").click(function () {

		var id = getId(this);

		getMessage(id);

		return false;

	});

}
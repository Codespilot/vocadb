
function getMessage(messageId) {

	$.get("../../User/Message", { messageId: messageId }, function (content) {

		$("#viewMessage").show();
		$("#viewMessage").html(content);

		$("#replyMessage").click(function () {

			var senderName = $("#senderName").val();
			$("#receiverName").val(senderName);

			var subject = $("#subject").val();
			$("#newMessageSubject").val("Re: " + subject);

			$("#tabs").tabs("select", "composeTab");

			return false;

		});

	});

}

function initPage() {

	$("#tabs").tabs();

	$("#receiverName").autocomplete({
		source: "../../User/FindByName"
	});

	$(".viewMessage").click(function () {

		var id = getId(this);

		getMessage(id);

		return false;

	});

}
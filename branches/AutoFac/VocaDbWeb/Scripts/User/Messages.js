
function getMessage(messageId) {

	$.get("../../User/Message", { messageId: messageId }, function (content) {

		$("#viewMessage").show();
		$("#viewMessage").html(content);

		$("#replyMessage").click(function () {

			var senderName = $("#senderName").val();
			$("#receiverName").val(senderName);

			var subject = $("#subject").val();
			$("#newMessageSubject").val("Re: " + subject);

			var index = $('#tabs ul').index($('#composeTab'));
			$("#tabs").tabs("option", "active", index);

			return false;

		});

	});

}

function initPage() {

	$("#tabs").tabs();

	$("#receiverName").autocomplete({
		source: "../../User/FindByName"
	});

	$("table.messages tr").click(function () {

		$(this).parent().find("tr").removeClass("info");
		$(this).addClass("info");

		var id = getId(this);

		getMessage(id);

		return false;

	});

}
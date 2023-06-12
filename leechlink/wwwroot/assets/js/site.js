$(document).ready(function () {
    // Khi button được nhấn
    $("#actionDownload").click(function () {
        // Lấy giá trị chuỗi string cần gửi
        var input = $("#urlFileInput")
        var inputString = input.val();
        if (!checkLink(inputString)) return;
        var parent = input.parent()
        parent.attr('disabled', 'disabled');
        parent.append('<label for="floatingInput" class="floatingInput-loading"> <div class= "spinner-border spinner-border-lg text-primary" role = "status" ></div></label> ')
        // Tạo Ajax request
        $.ajax({
            url: "/home/GetLink",
            type: "GET",
            dataType: "json",
            data: { url: inputString },
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                console.log(response, 'response')
                // Xử lý kết quả trả về từ action
                var resultString = response.fileUrl;
                if (resultString == "" || resultString == null) {
                    $('.error-validate').text("File not found");
                }
                $("#downfile").attr("href", resultString)
                $("#downfile").show();
                parent.removeAttr('disabled');
                $(".floatingInput-loading").remove();
            },
            error: function (xhr, status, error) {
                // Xử lý lỗi (nếu có)
                $('.error-validate').text("ERROR");
                parent.removeAttr('disabled');
                $(".floatingInput-loading").remove();
            }
        });
    });
    $("#btnclearLink").click(function () {
        $("#urlFileInput").val('')
        $("#urlFileInput").parent().removeAttr('disabled');
        $(".floatingInput-loading").remove();
        $('.error-validate').text("");
    })
});

function checkLink(inputString) {
    if (inputString != null && inputString.trim() != '') {
        return true;
    }
    return false
}

function onHiden($event) {
    $("#downfile").hide();
    $("#urlFileInput").val("")
}

$(document).ready(function () {
  // Khi button được nhấn
  $("#actionDownload").click(function () {
    // Lấy giá trị chuỗi string cần gửi
    var inputString = $("#urlFileInput").val();

    // Tạo Ajax request
    $.ajax({
      type: 'GET',
      dataType: 'application/json',
      url: @Url.Action("Home", "GetLink"),
    data: { urls: inputString },
    success: function (response) {
      // Xử lý kết quả trả về từ action
      var resultString = response.resultString;

      // Hiển thị kết quả trên trang
      alert("Kết quả: " + resultString);
    },
    error: function (xhr, status, error) {
      // Xử lý lỗi (nếu có)
      console.error(error);
    }
  });
});
});
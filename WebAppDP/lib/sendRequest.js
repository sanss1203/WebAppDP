$(document).ready(function () {
            $("#myForm").submit(function (event) {
                event.preventDefault();
                var formData = new FormData(this);
                
                var url = $("div[data-action-url]").data("action-url");

                Swal.fire({
                    title: "Buat Pesanan",
                    text: "Apakah anda yakin ingin membuat pesanan ini?",
                    icon: "question",
                    showCancelButton: true,
                    confirmButtonText: "Ya, Buat Pesanan",
                    cancelButtonText: "Batal",
                }).then((result) => {
                    if (result.isConfirmed) {
                        $('#loadingSpinner').show();
                        $.ajax({
                            url: $(this).attr("action"),
                            type: $(this).attr("method"),
                            data: formData,
                            processData: false,
                            contentType: false,
                            success: function (data) {
                                $('#loadingSpinner').show();
                                if (data.message == "Pesanan berhasil disimpan") {
                                    Swal.fire("Sukses", data.message, "success").then((result) => {
                                        if (result.isConfirmed) {
                                            window.location.href = data.redirectUrl;
                                        }
                                    });
                                }
                            },
                            error: function (xhr, status, error) {
                                Swal.fire({
                                    icon: "error",
                                    title: "Error",
                                    text: "Gagal membuat pesanan. Silahkan coba lagi nanti.",
                                });
                            },
                            complete: function () {
                                $('#loadingSpinner').fadeOut();
                            },
                        });
                    }
                });
            });


            
        });
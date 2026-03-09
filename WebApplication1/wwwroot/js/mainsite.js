// all dashboard categories performed here
$(document).ready(function () {
    var custId;
    var getId;
    var evalueId;
    //pie chart
    $.ajax({
        type: "GET",
        url: "/Dashboard/GetPieChartSummary",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            console.log("checking pie ", response);

            var xValues = response.categories; //["Italy", "France", "Spain", "USA", "Argentina","Nigeria"];
            var yValues = response.values;//[55, 49, 44, 24];
            var barColors = [
                "#b91d47",
                "#00aba9",
                "#2b5797",
                "#e8c3b9",
                "#1e7145"
            ];

            new Chart("myChart", {
                type: "pie",
                data: {
                    labels: xValues,
                    datasets: [{
                        backgroundColor: barColors,
                        data: yValues
                    }]
                },
                options: {
                    title: {
                        display: true,
                        text: "Customer Sources"
                    }
                }
            });

            // another ajax call - get ticket by branch
            $.ajax({
                type: "GET",
                url: "/Dashboard/GetTicketsbyBranchSummary",
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    console.log("checking bar chart ", response);

                    var xValues = response.branches;//["Italy", "France", "Spain", "USA", "Argentina"];
                    var yValues = response.values;//[55, 49, 44, 24, 15];
                    var barColors = ["red", "green", "blue", "orange", "brown", "black"];

                    new Chart("myBarChart", {
                        type: "bar",
                        data: {
                            labels: xValues,
                            datasets: [{
                                backgroundColor: barColors,
                                data: yValues
                            }]
                        },
                        options: {
                            legend: { display: false },
                            title: {
                                display: true,
                                text: "Tickets By Branch"
                            }
                        }
                    });

                    // call for the lastest top 10 records here
                    $.ajax({
                        type: "GET",
                        url: "/Dashboard/GetTop10TicketRecords",
                        data: {},
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            console.log("checking top 5", response);
                            $.each(response, function (key, value) {
                                $('#LatestTicket').append('<tr>  <td>' + value.AccountNo + '</td><td>' + value.Branch + '</td> <td>' + value.Category + '</td><td>' + value.ComType + '</td><td>' + value.DateLogged + '</td><td>' + value.Description + '</td><td>' + value.Status + '</td></tr>');

                            });

                            // get the ticket total count
                            $.ajax({
                                type: "GET",
                                url: "/Dashboard/GetTicketsTotalCount",
                                data: {},
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (response) {
                                    console.log ("GetTicketsTotalCount", response);
                                    $.each(response, function (key, value) {
                                        $('#TicketCount').append('<tr>  <td><a>TOTAL NO</a></td><td>' + value.NumberOfCount + '</td> </tr>');
                                    });
                                },
                            });
                        },
                    });

                },

            });

        },
    });




    // Ticket summary
    $.ajax({
        type: "GET",
        url: "/Dashboard/GetTicketsSummary",
        contentType: "application/json; charset=utf-8",
        beforeSend: function (xhr) {
            $("#TicketSummary").empty();
        },
        success: function (res) {
            console.log("checking model ", res)
            $.each(res, function (key, value) {

                console.log("key ", key);
                console.log("value ", value);
                $('#TicketSummary').append('<tr>  <td>' + value.Status + '</td> <td>' + value.NumberOfTickets + '</td></tr>');

                // another ajax call- get customer sources in pie chart

            });

        },
        failure: function (res) {
            console.log("errror1", res);
            toastr.error("Ticket summary not fetched");
        },
        error: function (res) {
            console.log("errror2", res);
            toastr.error("Ticket summary not fetched");

        }
    });




    $("#AutoPopulatedControl").hide();
    $("#AcctHistoryTable").hide();


    $("#AccountNo").on('input', function () {
        //alert("ok now");
        if ($(this).val().length === 10) {
            var model = {};
            model = $("#AccountNo").val();
            $.ajax({
                type: "POST",
                url: "/Accounts/GetAcctDetails",
                data: JSON.stringify({ accountNo: model }),

                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    if (res.accountNumber == null) {
                        $("#AutoPopulatedControl").hide();
                        toastr.error("Invalid AccountNumber");
                        $("#AccountNo").val("");
                    } else {
                        $("#AutoPopulatedControl").show();
                        $("#Name").val(res.accountName);
                        $("#Email").val(res.email);
                        $("#Phone").val(res.mainPhone);
                    }


                },
                failure: function (res) {
                    console.log("errror1", res.responseText);
                    toastr.error("Invalid AccountNumber");
                },
                error: function (res) {
                    console.log("errror2", res.responseText);
                    toastr.error("Invalid AccountNumber");

                }
            });

        }
    });


    // Get Ticket History
    $("#AccountNoForTicket").on('input', function () {
        if ($(this).val().length === 10) {
            var model = {};
            model = $("#AccountNoForTicket").val();
            console.log("checkMod ", model);
            $.ajax({
                type: "POST",
                url: "/Dashboard/GetTicketDetails",
                data: JSON.stringify({ accountNo: model }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr) {
                    $("#customerDatatable").empty();
                },
                success: function (res) {
                    console.log("ok again", res);
                    if (res.length == 0) {
                        $("#AcctHistoryTable").hide();
                        toastr.error("Invalid AccountNumber");
                    }
                    else {
                        $.each(res, function (key, value) {
                            console.log("ok again", value.AccountNo);
                            //if (value.AccountNo == null) {
                            //    $("#AcctHistoryTable").hide();
                            //    toastr.error("Invalid AccountNumber");
                            //} else {

                            $("#AcctHistoryTable").show();
                            // $("#myTable thead").prependTo("#customerDatatable")
                            console.log("idd", value.Id);
                            $('#customerDatatable').append('<tr> <td>' + value.AccountNo + '</td>  <td>' + value.Branch + '</td> <td>' + value.Category + '</td> <td>' + value.ComType + '</td> <td>' + value.Status + '</td> <td>' + value.Title + '</td> <td>' + value.Description + '</td><td>' + value.TicketNo + '</td><td>' + value.Comments + '</td><td><button type="button" id="' + value.Id + '" class="btn btn-primary pay_bill p-1" style="font-size:10px;" >update status</button></td></tr>');


                            //}


                        });
                    }



                },
                failure: function (res) {
                    console.log("errror1", res);
                    toastr.error("No record fetched");
                },
                error: function (res) {
                    console.log("errror2", res);
                    toastr.error("No record fetched");

                }
            });

        }
    });


    //
    $(document).on('click', '.pay_bill', function () {


        $(this).removeAttr("href");
        custId = $(this).attr("id");
        /*console.log("check1", custId)*/

        $.ajax({
            type: "POST",
            url: "/Tickets/EditPage",
            data: JSON.stringify({ id: custId }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                $.each(response, function (key, value) {
                    /*console.log("AccountNo ", value);*/
                    $("#upAccountNo").val(value.AccountNo);
                    $("#IncTicketNo").val(value.TicketNo);
                    getId = value.Id;
                    iniDocu = value.DocPath;
                    if (iniDocu == null) {
                        ////console.log("Docpath ", iniDocu);
                        ////var docUrl = window.location.protocol + '//' + window.location.host + '/' + Uploads/noimage.png.replace(/\\/g, "/");
                        ////console.log("url ", docUrl);

                        ////$("#target iframe").attr("src", docUrl);
                    }
                    else {
                        console.log("Docpath ", iniDocu);

                        //$("#Doc").val(iniDocu)
                        var docUrl = window.location.protocol + '//' + window.location.host + '/' + iniDocu.replace(/\\/g, "/");
                        console.log("url ", docUrl);

                        $("#target iframe").attr("src", docUrl);
                    }
                   
                });
                /*console.log("ok1 ", response);*/

                $("#myModal").modal('show');
            }
        });
    });

    //update
    $("#btnUpdate").click(function () {


        if ($.trim($("#statusId").val()) == "") {
            toastr.error("Please Select Status")
            return false
        }
        else if ($.trim($("#comments").val()) == "") {
            toastr.error("Please Enter Comment")
            return false
        }

        var status = $("#statusId").val()
        var acctNumber = $("#upAccountNo").val()
        var ticketNo = $("#IncTicketNo").val()
        var comment = $("#comments").val()

        console.log("check2", custId)
        console.log("getid", getId)

        var model = {

            Id: getId,
            Status: status,
            AccountNo: acctNumber,
            TicketNo: ticketNo,
            Comments:comment

        };

        //console.log("model3", model)

        $.ajax({
            type: "POST",
            url: "/Accounts/UpdateCustomerInformation",
            //data: JSON.stringify(model),
            data: JSON.stringify(model),

            contentType: "application/json",
            //dataType: "json",
            success: function (res) {
                $("#statusId").val("");
                $("#comments").val("");
               /* console.log("check response ", res);*/
                toastr.success(res);
                setTimeout(function () {

                    location.reload();
                }, 4000)
            },
            failure: function (res) {
               /* console.log("errror1", res);*/
                toastr.error(res);
            },
            error: function (res) {
               /* console.log("errror2", res.responseText);*/
                toastr.error(res.responseText);

            }
        });

    });

    //
    $("#btnRegisterUser").click(function () {


        if ($.trim($("#staffemail").val()) == "") {
            toastr.error("Please enter staffemail")
            return false
        }
        else if ($.trim($("#Password").val()) == "") {
            toastr.error("Enter Password")
            return false
        }
        else if ($.trim($("#staffName").val()) == "") {
            toastr.error("Enter StaffName")
            return false
        }
        else if ($.trim($("#branch").val()) == "") {
            toastr.error("select branch")
            return false
        }
        
        

        var sPassword = $("#Password").val()
        var sName = $("#staffName").val()
        var semail = $("#staffemail").val()
        var sBranch = $("#branch").val()


        var model = {

            password: sPassword,
            name: sName,
            staffemail: semail,
            last_login_date: "",
            status: 1,
            branch: sBranch

        };

        /*console.log("modelObj", model);*/

        $.ajax({
            type: "POST",
            url: "/Logins/SaveUserInformation",
            data: JSON.stringify(model),

            contentType: "application/json",
            //dataType: "json",
            success: function (res) {

                //console.log("check response ", res);
                toastr.success(res);
                //setTimeout(function () {
                //    location.reload();
                //}, 4000)
                // call for the lastest top 10 records here
                $.ajax({
                    type: "GET",
                    url: "/Logins/UserList",
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                      /*  console.log("checking user list", response);*/
                        $.each(response, function (key, value) {

                            // show table
                            $("#userList").show();
                            $('#userListDatatable').append('<tr>  <td>' + value.staffemail + '</td><td>' + value.name + '</td><td><button type="button" id="' + value.Id + '" class="btn btn-primary edit_user p-1" style="font-size:10px;" >edit</button></td></tr>');

                        });
                    },
                });




            },
            failure: function (res) {
                //console.log("errror1", res);
                toastr.error(res);
            },
            error: function (res) {
                console.log("errror2", res.responseText);
                toastr.error(res.responseText);

            }
        });


    });

    $("#showUserList").click(function () {
        //$("#userList").show();
        $.ajax({
            type: "GET",
            url: "/Logins/UserList",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr) {
                $("#userListDatatable").empty();
            },
            success: function (response) {

                //setTimeout(function () {
                //    location.reload();
                //}, 20000)
                //console.log("checking user list", response);
                $.each(response, function (key, value) {

                    // show table
                    $("#userList").show();
                    $('#userListDatatable').append('<tr>  <td>' + value.staffemail + '</td><td>' + value.name + '</td><td><button type="button" id="' + value.Id + '" class="btn btn-primary edit_user p-1" style="font-size:10px;" >edit</button></td></tr>');

                });
            },
        });
    });


    //edit user
    $(document).on('click', '.edit_user', function () {

        
        $(this).removeAttr("href");
        evalueId = $(this).attr("id");
        console.log("evalueId", evalueId);



        $("#myModal").modal('show');

        $.ajax({
            type: "POST",
            url: "/Logins/EditUser",
            data: JSON.stringify({ id: evalueId }),
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                $.each(response, function (key, value) {
                    console.log("AccountNo ", value);
                    $("#sName").val(value.name);
                    $("#sEmail").val(value.staffemail);                 
                    evalueId = value.Id;
                });               

              /*  $("#myUserModal").modal('show');*/
            }
        });
    });

    //update user
    $("#updateUser").click(function () {

        if ($.trim($("#LockUser").val()) == "") {
            toastr.error("Select to lock or unlock")
            return false
        }

        var sName = $("#sName").val()
        var eEmail = $("#sEmail").val()
        var stat = $("#LockUser").val()

        var model = {

            //Id: parseInt(evalueId),
            staffemail: eEmail,
            name: sName,
            status: stat

        };

        $.ajax({
            type: "POST",
            url: "/Logins/UpdateUserInformation",
            data: JSON.stringify(model),

            contentType: "application/json",
            //dataType: "json",
            success: function (res) {
               

                /* console.log("check response ", res);*/
                toastr.success(res);
                setTimeout(function () {

                    location.reload();
                }, 4000)
            },
            failure: function (res) {
                /* console.log("errror1", res);*/
                toastr.error(res);
            },
            error: function (res) {
                /* console.log("errror2", res.responseText);*/
                toastr.error(res);

            }
        });

    });


    // login
    $("#btnLogin").click(function () {

        if ($.trim($("#txtusername").val()) == "") {
            toastr.error("Enter staff email")
            return false
        }
        else if ($.trim($("#txtpassword").val()) == "") {
            toastr.error("Enter Password")
            return false
        }

        var user = $("#txtusername").val()
        var pass = $("#txtpassword").val()


        var model = {

            StaffEmail: user,
            Password: pass
            //Status: ""
        };
        console.log('checking user model', model);
        $.ajax({
            type: "POST",
            url: "/Logins/Login",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(model),
            //beforeSend: function (xhr) {
            //    $("#TicketSummary").empty();
            //},
            success: function (response) {

                if (response.length === 0) {
                    toastr.error("Email or password provided is incorrect.");
                    $("#txtpassword").val("");
                }
                else {

                    jQuery.each(response, function (i, val) {

                        if (val.password == 'password') {
                            $("#changePassword").show();
                            $("#loginCred").hide();
                            return;
                        }
                        else {
                            if (val.status == 1) {
                                nameId = val.Name;
                                window.location.href = '/Dashboard/Index/';
                            }
                            else {
                                toastr.error("Your account is locked.");
                            }
                        }

                        //if (val.status == 1) {
                        //    console.log("status ", response.status);
                        //    //toastr.success("correct");
                        //    window.location.href = '/Dashboard/Index/';
                        //}
                        //else {
                        //    toastr.error("Your account is locked.");
                        //}
                    });



                }
            },
            failure: function (res) {
                console.log("errror1", res);
                toastr.error("Login failed");
            },
            error: function (res) {
                console.log("errror2", res);
                toastr.error("Login failed");

            }
        });
    });


    $("#complaintType").change(function () {
        $("#Title").val($("#complaintType").val());
        $("#Title").prop("readonly", true);
    });

    // save record to DB
    //$(".mr-2").click(function () {
    $("#btnSubmit").click(function () {
        if ($.trim($("#category").val()) == "") {
            toastr.error("Please select category")
            return false
        }
        else if ($.trim($("#AccountNo").val()) == "") {
            toastr.error("Please enter AccountNo")
            return false
        }
        else if ($.trim($("#branch").val()) == "") {
            toastr.error("Please select branch")
            return false
        }
        else if ($.trim($("#complaintType").val()) == "") {
            toastr.error("Please select complaintType")
            return false
        }
        else if ($.trim($("#Title").val()) == "") {
            toastr.error("Please enter Title")
            return false
        }
        else if ($.trim($("#description").val()) == "") {
            toastr.error("Please enter description")
            return false
        }

        var category = $("#category").val()
        var acctNo = $("#AccountNo").val()
        var branch = $("#branch").val()
        var comType = $("#complaintType").val()
        var title = $("#Title").val()
        var desc = $("#description").val()
        var acctName = $("#Name").val()


        var file1 = document.getElementById("FormFile").files[0].name;

        var DocusModel = [
            { fileName: file1, fileType: file1.split(".")[1], file: $("#FormFile").data("imageUrl").replace(/^data:image\/[a-z]+;base64,/, "") },
        ];

        var model = {

            Category: category,
            AccountNo: acctNo,
            Branch: branch,
            ComType: comType,
            Title: title,
            Description: desc,
            AccountName: acctName,
            docus: DocusModel,

        };

        console.log("personal savings ", model);

        $.ajax({
            type: "POST",
            url: "/Accounts/SaveCustomerInformation",
            data: JSON.stringify(model),

            contentType: "application/json",
            //dataType: "json",
            success: function (res) {

                console.log("check response ", res);
                toastr.success(res);
                $("#Title").val("");
                $("#description").val("");
                $("#AccountNo").val("");
               
                setTimeout(function () {

                    location.reload();
                }, 4000)
            },
            failure: function (res) {
                console.log("errror1", res);
                toastr.error(res);
            },
            error: function (res) {
                console.log("errror2", res.responseText);
                toastr.error(res.responseText);

            }
        });
    });





    //function that convert to base64
    window.encodeImgtoBase64 = (element) => {
        var img = element.files[0];
        var reader = new FileReader();
        reader.onloadend = function () {
            $(element).data("imageUrl", reader.result);
            return reader.result;

        };
        reader.readAsDataURL(img);
    };


    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "5000",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

});








// open Tickets
$(document).ready(function () {
    //$('#table_id')
    //    .DataTable();
    $('#GetopenTickets').DataTable();
});
// closed
$(document).ready(function () {
    //$('#table_id')
    //    .DataTable();
    $('#GetclosedTickets').DataTable();
});
//inprogress
$(document).ready(function () {
    //$('#table_id')
    //    .DataTable();
    $('#GetInProgressTickets').DataTable();
});
//ticketlist
$(document).ready(function () {
    //$('#table_id')
    //    .DataTable();
    $('#GetTicketList').DataTable();
});

//onhold-ticket
$(document).ready(function () {
    //$('#table_id')
    //    .DataTable();
    $('#GetonholdTickets').DataTable();
});



//open tickets excel download
$("#ExportOpenTicketToExcel").click(function () {
    $("#GetopenTickets").table2excel({
        filename: "open_tickets.xls"
    });
});

//closed tickets excel download
$("#ExportClosedTicketToExcel").click(function () {
    $("#GetclosedTickets").table2excel({
        filename: "closed_tickets.xls"
    });
});

//inprogress tickets excel download
$("#ExportInprogressTicketToExcel").click(function () {
    $("#GetInProgressTickets").table2excel({
        filename: "Inprogress_tickets.xls"
    });
});
//ticketlist tickets excel download
$("#ExportTicketListToExcel").click(function () {
    $("#GetTicketList").table2excel({
        filename: "ticketslist.xls"
    });
});
//onhold tickets excel download
$("#ExportOnholdTicketToExcel").click(function () {
    $("#GetonholdTickets").table2excel({
        filename: "on-hold-ticket-list.xls"
    });
});


//forgot pass
$("#forgotpass").click(function () {
    /* alert("seen");*/
    $("#forgotpassword").show();
    $("#changePassword").hide();
    $("#loginCred").hide();
});
$("#btnForgotpass").click(function () {


    if ($.trim($("#txtusername").val()) == "") {
        toastr.error("Enter staff email")
        return false
    }

    var user = $("#txtusername").val()

    var model = {

        staffemail: user

    };

    $.ajax({
        type: "POST",
        url: "/Logins/PasswordReset",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(model),
        //beforeSend: function (xhr) {
        //    $("#TicketSummary").empty();
        //},
        success: function (response) {
            console.log("checking res ", response);
            //if (response == 2) {
            if (response == 1) {
                $("#forgotpassword").hide();
                $("#loginCred").show();
                toastr.success("Your password reset to default is successful. check your email and relogin");
                //$("#password").val() == "";
            }
            else if (response == 0) {
                toastr.error("Email provided is wrong");
            }
            else {

            }
        },
        failure: function (res) {
            console.log("errror1", res);
            toastr.error("password reset failed");
        },
        error: function (res) {
            console.log("errror2", res);
            toastr.error("password reset failed");

        }
    });


});



//change password
$(".passchange").click(function () {

    if ($.trim($("#txtusername").val()) == "") {
        toastr.error("Enter staff email")
        return false
    }
    else if ($.trim($("#NewPassword").val()) == "") {
        toastr.error("Enter New Password")
        return false
    }
    else if ($.trim($("#ComfirmNewPassword").val()) == "") {
        toastr.error("Enter Comfirm Password")
        return false
    }

    var user = $("#txtusername").val()
    var newpass = $("#NewPassword").val()
    var confirmnewpass = $("#ComfirmNewPassword").val()


    console.log("new ", newpass)
    console.log("confirm ", confirmnewpass)

    if (newpass != confirmnewpass) {
        toastr.error("The new password and confirm password does not match.");
        return;
    }
    else if (newpass.length < 8) {
        toastr.error("Your password is less than 8 character");
        return;
    }
    if (/^[a-zA-Z0-9- ]*$/.test(newpass) == true) {
        toastr.error('Your password MUST contain special characters.');
        return;
    }


    var model = {

        staffemail: user,
        password: newpass,
        ComfirmNewPassword: confirmnewpass
        //Status: ""
    };

    console.log('checking user model', model);
    $.ajax({
        type: "POST",
        url: "/Logins/NewPassword",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(model),
        dataType: "json",
        success: function (response) {
            console.log("res ", response);
            if (response != null) {
                console.log("status ", response.status);
                //toastr.success("correct");
                //window.location.href = '/Dashboard/Index/';
                $("#changePassword").hide();
                $("#loginCred").show();
            }
            else {
                toastr.error("New password creation failed");
            }

        },
        failure: function (res) {
            console.log("errror1", res);
            toastr.error("Login failed");
        },
        error: function (res) {
            console.log("errror2", res);
            toastr.error("Login failed");

        }
    });
});






//(function ($) {

//})(jQuery);


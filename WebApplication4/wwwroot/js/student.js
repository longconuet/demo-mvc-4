//Load Data in Table when documents is ready  
$(document).ready(function () {
    var t = $("#demoGrid").DataTable({
        "processing": true, // for show progress bar  
        "serverSide": true, // for process server side  
        "filter": true, // this is for disable filter (search box)  
        "orderMulti": false, // for disable multiple column at once  
        "pageLength": 2,
        "lengthMenu": [2, 5, 10, 25],
        "searchDelay": 500,
        
        "ajax": {
            "url": "/Student/LoadData",
            "type": "POST",
            "datatype": "json"
        },

        "columnDefs":
            [{
                "targets": [1],
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [3],
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [4],
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [5],
                "searchable": false,
                "orderable": false
                }],

        "columns": [
            { "data": "fullName", "name": "fullName", "autoWidth": true },
            { "data": "code", "name": "code", "autoWidth": true },
            { "data": "age", "name": "age", "autoWidth": true },
            { "data": "address", "name": "address", "autoWidth": true },
            {
                data: null, render: function (data, type, row) {
                    var html = '';
                    $.each(row.courses, function (key, item) {
                        html += "<div><a href='#' onclick='return getCourseInfo(" + item.id + ")'>" + item.name + "</a></div>";
                    });
                    return html;
                }
            },
            {
                "render": function (data, type, full, meta) {
                    return '<td><a href="#" onclick="return getbyID(' + full.id + ')">Edit</a> | <a href="#" onclick="Delele(' + full.id + ')">Delete</a></td>';
                }
            },
        ],
    });

    var validator = $("#form").validate({
        rules: {
            fullName: {
                required: true,
                minlength: 5,
                maxlength: 50
            },
            code: {
                required: true,
                maxlength: 50
            },
            age: {
                required: true,
                min: 1,
                max: 200
            }
        },

        messages: {
            fullName: {
                required: "Name cannot be empty",
                minlength: "Name must has at least 5 characters",
                maxlength: "Name must be less than or equal 50 characters",
            },
            code: {
                required: "Code cannot be empty",
                maxlength: "Code must be less than or equal 50 characters",
            },
            age: {
                required: "Age cannot be empty",
                min: "Age must be great than 0",
                max: "Age must be less than or equal 200",
            }
        }
    });

});

//Load Data function  
function loadData(txtSearch = "", page = 1) {
    $.ajax({
        url: "/Student/List",
        type: "GET",
        data: { keyword: txtSearch, page: page },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {

            var html = '';
            $.each(result.students, function (key, item) {
                var htmlCourse = '';
                $.each(item.courses, function (keyCourse, courseName) {
                    htmlCourse += '<div>' + courseName + '</div>';
                });

                html += '<tr>';
                html += '<td>' + (key + 1) + '</td>';
                html += '<td>' + item.fullName + '</td>';
                html += '<td>' + item.code + '</td>';
                html += '<td>' + item.age + '</td>';
                html += '<td>' + item.address + '</td>';
                html += '<td>' + htmlCourse + '</td>';
                html += '<td><a href="#" onclick="return getbyID(' + item.id + ')">Edit</a> | <a href="#" onclick="Delele(' + item.id + ')">Delete</a></td>';
                html += '</tr>';
            });
            $('.tbody').html(html);

            //create pagination
            var pagination_string = "";
            var pageCurrent = result.pageCurrent;
            var totalPage = result.totalPage;

            //create button previous 
            if (pageCurrent > 1) {
                var pagePrevious = pageCurrent - 1;
                pagination_string += '<li class="page-item"><a href="" class="page-link" data-page=' + pagePrevious + '>Previous</a></li>';
            }

            for (i = 1; i <= totalPage; i++) {
                if (i == pageCurrent) {
                    pagination_string += '<li class="page-item active"><a href="" class="page-link" data-page=' + i + '>' + pageCurrent + '</a></li>';
                } else {
                    pagination_string += '<li class="page-item"><a href="" class="page-link" data-page=' + i + '>' + i + '</a></li>';
                }
            }

            //create button next
            if (pageCurrent > 0 && pageCurrent < totalPage) {
                var pageNext = pageCurrent + 1;
                pagination_string += '<li class="page-item"><a href="" class="page-link"  data-page=' + pageNext + '>Next</a></li>';
            }

            //load pagination
            $("#load-pagination").html(pagination_string);

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}  

//click event pagination
$("body").on("click", ".pagination li a", function (event) {
    event.preventDefault();
    var page = $(this).attr('data-page');

    //load event pagination
    var txtSearch = $(".txtSearch").val();
    if (txtSearch != "") {
        loadData(txtSearch, page)
    }
    else {
        loadData(null, page);
    }

});

// search event
//$("body").on("click", "#search", function (event) {
//    event.preventDefault();

//    //load event pagination
//    var txtSearch = $(".txtSearch").val();
//    if (txtSearch != "") {
//        loadData(txtSearch, 1)
//    }
//    else {
//        loadData(null, 1);
//    }
//});

$("body").on("keyup", ".txtSearch", function (event) {
    event.preventDefault();

    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();

        //load event pagination
        var txtSearch = $(".txtSearch").val();
        if (txtSearch != "") {
            loadData(txtSearch, 1)
        }
        else {
            loadData(null, 1);
        }
    }
});


//Add Data Function   
function Add() {
    //var res = validate();
    //if (res == false) {
    //    return false;
    //}
    //e.preventDefault();

    if ($("#form").valid()) {
        var stdObj = {
            FullName: $('#FullName').val(),
            Age: $('#Age').val(),
            Code: $('#Code').val(),
            Address: $('#Address').val(),
        };

        $.ajax({
            url: "/Student/Create",
            data: JSON.stringify(stdObj),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            async: true,
            processData: false,
            statusCode: {
                400: function (responseObject, textStatus, jqXHR) {
                    toastr.error(responseObject, "Bad request");
                }
            },
            success: function (result) {
                if (result.status == 0) {
                    toastr.error(result.message, "Error");
                    return false;
                }

                toastr.success(result.message, 'Success');
                reloadTable();
                $('#myModal').modal('hide');
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
    
}

function reloadTable() {
    var currentPage = $("#demoGrid").DataTable().page();
    $("#demoGrid").DataTable().page(currentPage).draw('page');
}


function getbyID(id) {
    $('#Name').css('border-color', 'lightgrey');
    $('#Age').css('border-color', 'lightgrey');
    $.ajax({
        url: "/Student/GetById/" + id,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 1) {
                $('#Id').val(result.data.id);
                $('#FullName').val(result.data.fullName);
                $('#Age').val(result.data.age);
                $('#Code').val(result.data.code).attr('disabled', 'disabled');
                $('#Address').val(result.data.address);

                $('#btnUpdate').show();
                $('#btnAdd').hide();
                $('#myModal').modal('show');
            }
            else {
                toastr.error("Error", result.message);
                loadData();
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}

//function for updating
function Update() {
    //var res = validate();
    //if (res == false) {
    //    return false;
    //}

    var stdObj = {
        Id: $('#Id').val(),
        FullName: $('#FullName').val(),
        Age: $('#Age').val(),
        Code: $('#Code').val(),
        Address: $('#Address').val(),
    };

    $.ajax({
        url: "/Student/Update",
        data: JSON.stringify(stdObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 0) {
                toastr.error(result.message, "Error");
                loadData();
                return false;
            }

            toastr.success(result.message, 'Success');
            reloadTable();
            $('#myModal').modal('hide');
            clearTextBox();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//function for deleting
function Delele(Id) {
    var ans = confirm("Are you sure you want to delete this Record?");
    if (ans) {
        $.ajax({
            url: "/Student/Delete/" + Id,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                if (result.status == 0) {
                    toastr.error(result.message, "Error");
                    loadData();
                    return false;
                }

                toastr.success(result.message, 'Success');
                reloadTable();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
}  


function getCourseInfo(id) {
    $.ajax({
        url: "/Student/GetCourseInfo/" + id,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 1) {
                $('#course-name').val(result.data.name);
                $('#course-code').val(result.data.code);
                $('#course-max-student-num').val(result.data.maxStudentNum);
                $('#courseModal').modal('show');
            }
            else {
                toastr.error(result.message, "Error");
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}

function showCourseModal() {
    clearTextBox();
    $('#courseModal').modal('show');
}

function hideCourseModal() {
    $('#courseModal').modal('hide');
}

//Function for clearing the textboxes  
function clearTextBox() {
    $('#Id').val("");
    $('#FullName').val("");
    $('#Age').val("");
    $('#Code').val("");
    $('#Address').val("");
    $('#btnUpdate').hide();
    $('#btnAdd').show();
    $('#FullName').css('border-color', 'lightgrey');
    $('#Age').css('border-color', 'lightgrey');
    $('#Code').css('border-color', 'lightgrey');
    $('#Address').css('border-color', 'lightgrey');
}

function showAddModal() {
    clearTextBox();
    $('#myModal').modal('show');
}

function hideModal() {
    $('#myModal').modal('hide');
}

//Valdidation using jquery  
function validate() {
    var isValid = true;
    if ($('#Name').val().trim() == "") {
        $('#Name').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Name').css('border-color', 'lightgrey');
    }
    if ($('#Age').val().trim() == "") {
        $('#Age').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Age').css('border-color', 'lightgrey');
    }

    return isValid;
}  
﻿//Load Data in Table when documents is ready  
$(document).ready(function () {
    loadData();

    var validator = $("#form").validate({
        rules: {
            name: {
                required: true,
                minlength: 5,
                maxlength: 50
            },
            code: {
                required: true,
                maxlength: 50
            },
            maxStudentNum: {
                required: true,
                min: 1,
                max: 200
            }
        },

        messages: {
            name: {
                required: "Name cannot be empty",
                minlength: "Name must has at least 5 characters",
                maxlength: "Name must be less than or equal 50 characters",
            },
            code: {
                required: "Code cannot be empty",
                maxlength: "Code must be less than or equal 50 characters",
            },
            maxStudentNum: {
                required: "Max student number cannot be empty",
                min: "Max student number must be great than 0",
                max: "Max student number must be less than or equal 200",
            }
        }
    });

    $("#form-enroll").validate({
        rules: {
            'studentEnroll[]': {
                required: true
            },
        },

        messages: {
            'studentEnroll[]': {
                required: "Select at least 1 student"
            }
        }
    });

});

//Load Data function  
function loadData(txtSearch = "") {
    $.ajax({
        url: "/Course/List",
        type: "GET",
        data: { keyword: txtSearch },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<tr>';
                html += '<td>' + (key + 1) + '</td>';
                html += '<td>' + item.name + '</td>';
                html += '<td>' + item.code + '</td>';
                html += '<td><a href="#" onclick="showStudentModal(' + item.id + ')">' + item.currentStudentNum + '/' + item.maxStudentNum + '</a></td>';
                html += '<td><a href="#" onclick="return getbyID(' + item.id + ')">Edit</a> | <a href="#" onclick="Delele(' + item.id + ')">Delete</a>| <a href="#" onclick="showEnrollModal(' + item.id + ')">Enroll</a></td>';
                html += '</tr>';
            });
            $('.tbody').html(html);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}  


// search event
//$('#search-course"').keypress(function (event) {
//    var keycode = (event.keyCode ? event.keyCode : event.which);
//    if (keycode == '13') {
//        event.preventDefault();

//        //load event pagination
//        loadData($(".txtSearch").val());
//    }
//});
$("body").on("keyup", "#search-course", function (event) {
    event.preventDefault();

    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();

        //load event pagination
        loadData($(".txtSearch").val());
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
        var obj = {
            Name: $('#Name').val(),
            MaxStudentNum: $('#MaxStudentNum').val(),
            Code: $('#Code').val(),
        };

        $.ajax({
            url: "/Course/Create",
            data: JSON.stringify(obj),
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
                loadData();
                $('#myModal').modal('hide');
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
    
}


function getbyID(id) {
    $('#Name').css('border-color', 'lightgrey');
    $('#MaxStudentNum').css('border-color', 'lightgrey');
    $.ajax({
        url: "/Course/GetById/" + id,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 1) {
                $('#Id').val(result.data.id);
                $('#Name').val(result.data.name);
                $('#MaxStudentNum').val(result.data.maxStudentNum);
                $('#Code').val(result.data.code).attr('disabled', 'disabled');

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

    var obj = {
        Id: $('#Id').val(),
        Name: $('#Name').val(),
        MaxStudentNum: $('#MaxStudentNum').val(),
        Code: $('#Code').val(),
    };

    $.ajax({
        url: "/Course/Update",
        data: JSON.stringify(obj),
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
            loadData();
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
            url: "/Course/Delete/" + Id,
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
                loadData();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
}  


//Function for clearing the textboxes  
function clearTextBox() {
    $('#Id').val("");
    $('#Name').val("");
    $('#MaxStudentNum').val("");
    $('#Code').val("");
    $('#btnUpdate').hide();
    $('#btnAdd').show();
    $('#Name').css('border-color', 'lightgrey');
    $('#MaxStudentNum').css('border-color', 'lightgrey');
    $('#Code').css('border-color', 'lightgrey');
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
    if ($('#MaxStudentNum').val().trim() == "") {
        $('#MaxStudentNum').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#MaxStudentNum').css('border-color', 'lightgrey');
    }

    return isValid;
}  


function showEnrollModal(courseId) {
    loadStudentDataToEnroll(courseId);
    $('#course-enroll-id').val(courseId);
    $('#modal-enroll').modal('show');
}

function hideEnrollModal() {
    $(".search-student-enroll").val("");
    $('#modal-enroll').modal('hide');
}

//Load Data function  
function loadStudentDataToEnroll(courseId, keyword = "") {
    $.ajax({
        url: "/Course/StudentsToEnroll",
        type: "GET",
        data: { id: courseId, keyword: keyword },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 0) {
                toastr.error(result.message, "Error");
                $('.tbody-enroll').html("");
                return false;
            }

            var data = result.data;
            var html = '';
            $.each(data, function (key, item) {
                html += '<tr>';
                html += '<td><input class="form-check-input" type="checkbox" name="studentEnrroll[]" value="' + item.id + '" id="student-' + item.id + '"></td>';
                html += '<td>' + item.fullName + '</td>';
                html += '<td>' + item.code + '</td>';
                html += '<td>' + item.age + '</td>';
                html += '</tr>';
            });
            $('.tbody-enroll').html(html);
        },
        error: function (errormessage) {
            $('.tbody-enroll').html("");
            alert(errormessage.responseText);
        }
    });
}


// search event
//$("body").on("click", "#search-student-enroll", function (event) {
//    event.preventDefault();

//    //load event pagination
//    loadStudentDataToEnroll(parseInt($('#course-enroll-id').val()), $(".search-student-enroll").val());
//});

$("body").on("keyup", ".search-student-enroll", function (event) {
    event.preventDefault();

    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();

        //load event pagination
        loadStudentDataToEnroll(parseInt($('#course-enroll-id').val()), $(".search-student-enroll").val());
    }
});


function enrollStudents() {
    if ($("#form-enroll").valid()) {
        var studentIds = [];
        $('#modal-enroll input:checked').each(function () {
            studentIds.push(parseInt($(this).val()));
        });

        var obj = {
            CourseId: parseInt($('#course-enroll-id').val()),
            StudentIds: studentIds
        };

        $.ajax({
            url: "/Course/EnrollStudentToCourse",
            data: JSON.stringify(obj),
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
                loadData();
                hideEnrollModal();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }

}



function showStudentModal(courseId) {
    loadStudentDataOfCourse(courseId);
    $('#course-student-id').val(courseId);
    $('#modal-students').modal('show');
}

function hideStudentModal() {
    $(".search-student-course").val("");
    $('#modal-students').modal('hide');
}

//Load Data function  
function loadStudentDataOfCourse(courseId, keyword = "") {
    $.ajax({
        url: "/Course/StudentsOfCourse",
        type: "GET",
        data: { id: courseId, keyword: keyword },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 0) {
                toastr.error(result.message, "Error");
                $('.tbody-student').html("");
                return false;
            }

            var data = result.data;
            var html = '';
            $.each(data, function (key, item) {
                html += '<tr>';
                html += '<td>' + (key + 1) + '</td>';
                html += '<td>' + item.fullName + '</td>';
                html += '<td>' + item.code + '</td>';
                html += '<td>' + item.age + '</td>';
                html += '<td><a href="#" onclick="removeStudent(' + courseId + ', ' + item.id + ')">Remove</a></td>';
                html += '</tr>';
            });
            $('.tbody-student').html(html);
        },
        error: function (errormessage) {
            $('.tbody-student').html("");
            alert(errormessage.responseText);
        }
    });
}


// search event
//$("body").on("click", "#search-student-course", function (event) {
//    event.preventDefault();

//    //load event pagination
//    loadStudentDataOfCourse(parseInt($('#course-student-id').val()), $(".search-student-course").val());
//});

$("body").on("keyup", ".search-student-course", function (event) {
    event.preventDefault();

    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        event.preventDefault();

        //load event pagination
        loadStudentDataOfCourse(parseInt($('#course-student-id').val()), $(".search-student-course").val());
    }
});


function removeStudent(courseId, studentId) {
    var ans = confirm("Are you sure you want to delete this Record?");
    if (ans) {
        $.ajax({
            url: "/Course/RemoveStudentFromCourse",
            data: JSON.stringify({ CourseId: courseId, StudentId: studentId }),
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
                loadStudentDataOfCourse(courseId);
                loadData();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }

}
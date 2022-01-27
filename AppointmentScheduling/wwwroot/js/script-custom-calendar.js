var routeURL = location.protocol + "//" + location.host;
$(document).ready(function () {
        $("#appointmentdate").kendoDateTimePicker({
            value: new Date(),
            dateInput: true
        });
        InitializeCalendar();
    }
);
var calendar;
function InitializeCalendar() {
    try {


        var calendarEl = document.getElementById('calendar');
        if (calendarEl != null) {
            calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                headerToolbar: {
                    left: 'prev,next,today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek,timeGridDay'
                },
                selectable: true,
                editable: false,
                select: function (event) {
                    onShowModal(event, null);
                },
                eventDisplay: 'block',
                events: function (fetchInfo, successCallback, failureCallback) {
                    $.ajax({
                        url: routeURL + '/api/Appointment/GetCalenderData?doctorId=' + $("#doctorId").val(),
                        type: 'GET',
                        dataType: 'JSON',
                        success: function (response) {
                            var events = [];
                            if (response.status === 1) {
                                $.each(response.dataenum, function (i, data) {
                                    events.push({
                                        title: data.title,
                                        description: data.description,
                                        start: data.startDate,
                                        end: data.endDate,
                                        backgroundColor: data.isDoctorApproved ? "#28a745" : "#dc3545",
                                        borderColor: "#162466",
                                        textColor: "white",
                                        id: data.id
                                    });
                                })
                            }
                            successCallback(events);
                        },
                        error: function (xhr) {
                            $.notify("Error", "error");
                        }
                    });
                },
                eventClick: function (info) {
                    getEventDetailsByEventId(info.event);
                }
            });
            calendar.render();
        }

    }
    catch (e) {
        alert(e);
    }

}

function onShowModal(obj, isEventDetails) {
    if (isEventDetails != null) {
        $("#Id").val(obj.id);
        $("#title").val(obj.title);
        $("#description").val(obj.description);
        $("#appointmentdate").val(obj.startDate);
        $("#doctorId").val(obj.doctorId);
        $("#patientID").val(obj.patientId);
        $("#durationid").val(obj.duration);
        $("#lblPatientName").html(obj.patientName);
        $("#lblDoctorName").html(obj.doctorName);
        if (obj.isDoctorApproved) {
            $("#lblStatus").html('Approved');
            $("#btnSubmit").addClass("d-none");
            $("#btnConfirm").addClass("d-none");
        }
        else {
            $("#lblStatus").html('Pending');
            $("#btnConfirm").removeClass("d-none");
            $("#btnSubmit").removeClass("d-none");
        }
        $("#btnDelete").removeClass("d-none");
        $("#btnSubmit").removeClass("d-none");
    }
    else {
        $("#appointmentdate").val(obj.startStr + " " + new moment().format("hh:mm A"));
        $("#Id").val(0);
        $("#btnDelete").addClass("d-none");
        $("#btnSubmit").removeClass("d-none");
    }
    $("#appointmentInput").modal("show");
}

function onCloseModal(obj, isEventDetails) {
    $("#appointmentForm")[0].reset();
    $("#Id").val(0);
    $("#title").val('');
    $("#description").val('');
    $("#appointmentdate").val('');
    $("#appointmentInput").modal("hide");
}

function onSubmitForm() {
    if (checkValidation()) {
        var requestData = {
            Id: parseInt($("#Id").val()),
            Title: $("#title").val(),
            Description: $("#description").val(),
            StartDate: $("#appointmentdate").val(),
            Duration: $("#durationid").val(),
            DoctorId: $("#doctorId").val(),
            PatientId: $("#patientID").val()
        };

        $.ajax({
            url: routeURL + '/api/Appointment/SaveCalenderData',
            type: 'POST',
            data: JSON.stringify(requestData),
            contentType: 'application/json',
            success: function (response) {
                if (response.status === 1 || response.status === 2) {
                    calendar.refetchEvents();
                    $.notify(response.message, "success");
                    onCloseModal();
                }
                else {
                    $.notify(response.message, "error");
                }
            },
            error: function (xhr) {
                $.notify("error", "errors");
            }
        });
    }
}

function checkValidation() {
    var IsValid = true;
    if ($("#title").val() === undefined || $("#title").val() === "") {
        IsValid = false;
        $("#title").addClass('error');
    }
    else {
        $("#title").removeClass('error');
    }

    if ($("#appointmentdate").val() === undefined || $("#appointmentdate").val() === "") {
        IsValid = false;
        $("#appointmentdate").addClass('error');
    }
    else {
        $("#appointmentdate").removeClass('error');
    }
    return IsValid;
}

function getEventDetailsByEventId(info) {
    $.ajax({
        url: routeURL + '/api/Appointment/GetCalendarDataById/' + info.id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {

            if (response.status === 1 && response.dataenum !== undefined) {
                onShowModal(response.dataenum, true);
            }
            successCallback(events);
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });
}

function onDoctorChange() {
    calendar.refetchEvents();
}

function onDeleteAppointment() {
    var id = parseInt($("#Id").val());
    $.ajax({
        url: routeURL + '/api/Appointment/DeleteAppointment/' + id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {

            if (response.status === 1) {
                $.notify(response.message, "success");
                calendar.refetchEvents();
                onCloseModal();
            }
            else {
                $.notify(response.message, "error");
            }
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });
}

function onConfirm() {
    var id = parseInt($("#Id").val());
    $.ajax({
        url: routeURL + '/api/Appointment/ConfirmEvent/' + id,
        type: 'GET',
        dataType: 'JSON',
        success: function (response) {

            if (response.status === 1) {
                $.notify(response.message, "success");
                calendar.refetchEvents();
                onCloseModal();
            }
            else {
                $.notify(response.message, "error");
            }
        },
        error: function (xhr) {
            $.notify("Error", "error");
        }
    });

}
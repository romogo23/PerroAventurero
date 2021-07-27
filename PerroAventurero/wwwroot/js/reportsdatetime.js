


function rangeDatetime() {

    var selectReport = document.getElementById("report").value;
    if (selectReport == 'Comparación entre asistencia y reservaciones por evento') {
        changeStyleBlock();

    } else if (selectReport == 'Comparación entre asistencia y reservaciones de todos los eventos') {
        changeStyleNone();

    } else if (selectReport == 'Comparación entre asistencia y reservaciones de niños') {
        changeStyleNone();


    } else if (selectReport == 'Top 10 usuarios que han asistido a más eventos') {
        changeStyleNone();

    } else if (selectReport == 'Promedio de edad de asistencia por evento') {
        changeStyleBlock();


    } else if (selectReport == 'Género de los asistentes por evento') {
        changeStyleBlock();

    } else if (selectReport == 'Género de asistentes') {
        changeStyleNone();

    }
     
    
}




function changeStyleBlock() {
    document.getElementById("start").style.display = 'block';
    document.getElementById("final").style.display = 'block';
}

function changeStyleNone() {
    document.getElementById("start").style.display = 'none';
    document.getElementById("final").style.display = 'none';
}

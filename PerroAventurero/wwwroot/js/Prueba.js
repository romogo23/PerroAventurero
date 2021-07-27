function TicketsAdults() {
    Price();
    adultTickets();

}



function TicketsChildrens() {
    Price();
    childTickets();

}



function Price() {
    var m1 = document.getElementById("entradasGenerales").value;
    var m2 = document.getElementById("entradasNinnos").value;
    r = m1 * 3500 + m2 * 2500;
    document.getElementById("price").innerHTML = r;
}

function adultTickets() {
    var m1 = document.getElementById("entradasGenerales").value;

    var divAge = document.getElementById("divAdultAge");
    if (divAge) {
        var doc = divAge.parentNode;
        doc.removeChild(divAge);
    }

    var divGen = document.getElementById("divAdultGen");
    if (divGen) {
        var doc = divGen.parentNode;
        doc.removeChild(divGen);
    }

    var divAge = document.createElement('div');
    divAge.setAttribute("id", "divAdultAge");

    var divGen = document.createElement('div');
    divGen.setAttribute("id", "divAdultGen");


    //var adLabel = document.createElement('label');
    //let adText = document.createTextNode("Adultos:");

    //adLabel.appendChild(adText); //preguntar
    //divAge.appendChild(adLabel);


    for (let i = 0; i < m1 - 1; i++) {
        var Age = document.createElement('input');
        Age.setAttribute("Type", "number");
        Age.setAttribute("name", "Age");
        Age.setAttribute("id", "Edad");

        var AgeLabel = document.createElement('label');
        let ageText = document.createTextNode("Edad");

        AgeLabel.appendChild(ageText);




        var genderLabel = document.createElement('label');
        let genderText = document.createTextNode("Género");
        genderLabel.appendChild(genderText);





        var gender = document.createElement('select');
        gender.setAttribute("id", "Gender");
        gender.setAttribute("name", "Gender");


        let genderMasculine = document.createElement("option");
        genderMasculine.setAttribute("value", "m");
        let genderMText = document.createTextNode("Masculino")
        genderMasculine.appendChild(genderMText);

        let genderFemenine = document.createElement("option");
        genderFemenine.setAttribute("value", "f");
        let genderFText = document.createTextNode("Femenino")
        genderFemenine.appendChild(genderFText);

        let genderOther = document.createElement("option");
        genderOther.setAttribute("value", "o");
        let genderOText = document.createTextNode("Otro")
        genderOther.appendChild(genderOText);


        gender.appendChild(genderFemenine);
        gender.appendChild(genderMasculine);
        gender.appendChild(genderOther);

        divAge.appendChild(AgeLabel);
        divAge.appendChild(Age);
        divGen.appendChild(genderLabel);
        divGen.appendChild(gender);


        document.getElementById("AdultTicketsAge").appendChild(divAge)
        document.getElementById("AdultTicketsGen").appendChild(divGen)


    }
}


function childTickets() {
    var m2 = document.getElementById("entradasNinnos").value;


    var divAge = document.getElementById("divChildAge");
    if (divAge) {
        var doc = divAge.parentNode;
        doc.removeChild(divAge);
    }
    var niLabel = document.createElement('label');
    let niText = document.createTextNode("Niños:");
    niLabel.appendChild(niText);

    var divAge = document.createElement('div');
    divAge.setAttribute("id", "divChildAge");



    for (let i = 0; i < m2; i++) {
        var Age = document.createElement('input');
        Age.setAttribute("Type", "number");
        Age.setAttribute("name", "Age");
        Age.setAttribute("id", "AgeChild");
        var AgeLabel = document.createElement('label');
        let ageText = document.createTextNode("Edad");
        AgeLabel.appendChild(ageText);
        var genderLabel = document.createElement('label');
        let genderText = document.createTextNode("Género");
        genderLabel.appendChild(genderText);





        var gender = document.createElement('select');
        gender.setAttribute("id", "GenderChild");
        gender.setAttribute("name", "Gender");


        let genderMasculine = document.createElement("option");
        genderMasculine.setAttribute("value", "m");
        let genderMText = document.createTextNode("Masculino")
        genderMasculine.appendChild(genderMText);

        let genderFemenine = document.createElement("option");
        genderFemenine.setAttribute("value", "f");
        let genderFText = document.createTextNode("Femenino")
        genderFemenine.appendChild(genderFText);

        let genderOther = document.createElement("option");
        genderOther.setAttribute("value", "o");
        let genderOText = document.createTextNode("Otro")
        genderOther.appendChild(genderOText);

        gender.appendChild(genderFemenine);
        gender.appendChild(genderMasculine);
        gender.appendChild(genderOther);


        divAge.appendChild(AgeLabel);
        divAge.appendChild(Age);
        divAge.appendChild(genderLabel);
        divAge.appendChild(gender);
        document.getElementById("ChildTickets").appendChild(divAge)
    }
}
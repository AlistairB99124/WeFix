$(function () {
    
    $(".datepicker").datepicker({
        autoclose: true
    });

    //$('input[type="checkbox"], input[type="radio"]').iCheck({
    //    checkboxClass: "icheckbox_minimal-blue",
    //    radioClass: "iradio_minimal-blue"
    //});
});

function InitiateSelect(sel) {
    var mySelect = document.createElement("SELECT");
    var myDiv = document.getElementById('myDiv');
    mySelect.id = "mySelect";
    mySelect.setAttribute("class", "form-control");
    mySelect.setAttribute("style", "width:100%;");
    myDiv.appendChild(mySelect);

    var defaultOption = document.createElement('OPTION');
    defaultOption.value = 0;
    defaultOption.text = 'Select';
    defaultOption.selected = true;
    mySelect.appendChild(defaultOption);
    mySelect.setAttribute("onchange", "SetSubCat();")

    switch (sel) {

        case 1:


            var option9 = document.createElement("OPTION");
            option9.value = "9";
            option9.text = "Littering";
            mySelect.appendChild(option9);


            var option10 = document.createElement("OPTION");
            option10.value = "10";
            option10.text = "Garbage Collection";
            mySelect.appendChild(option10);


            break;


        case 2:


            var option11 = document.createElement("OPTION");
            option11.value = "11";
            option11.text = "Stray";
            mySelect.appendChild(option11);


            var option12 = document.createElement("OPTION");
            option12.value = "12";
            option12.text = "Carcass";
            mySelect.appendChild(option12);


            break;


        case 3:


            var option13 = document.createElement("OPTION");
            option13.value = "13";
            option13.text = "Infrastracture";
            mySelect.appendChild(option13);


            var option14 = document.createElement("OPTION");
            option14.value = "14";
            option14.text = "Security";
            mySelect.appendChild(option14);


            var option15 = document.createElement("OPTION");
            option15.value = "15";
            option15.text = "Furnishing";
            mySelect.appendChild(option15);


            var option16 = document.createElement("OPTION");
            option16.value = "16";
            option16.text = "Vandalism";
            mySelect.appendChild(option16);


            break;


        case 4:


            break;


        case 5:


            var option17 = document.createElement("OPTION");
            option17.value = "17";
            option17.text = "Furnishing";
            mySelect.appendChild(option17);


            var option18 = document.createElement("OPTION");
            option18.value = "18";
            option18.text = "Floor";
            mySelect.appendChild(option18);


            var option19 = document.createElement("OPTION");
            option19.value = "19";
            option19.text = "Window";
            mySelect.appendChild(option19);


            break;


        case 6:


            var option1 = document.createElement("OPTION");
            option1.value = "1";
            option1.text = "Pot-holes";
            mySelect.appendChild(option1);


            var option2 = document.createElement("OPTION");
            option2.value = "2";
            option2.text = "Street-Signs";
            mySelect.appendChild(option2);


            var option3 = document.createElement("OPTION");
            option3.value = "3";
            option3.text = "Tree";
            mySelect.appendChild(option3);


            var option4 = document.createElement("OPTION");
            option4.value = "4";
            option4.text = "Floor";
            mySelect.appendChild(option4);


            var option5 = document.createElement("OPTION");
            option5.value = "5";
            option5.text = "Road Markings";
            mySelect.appendChild(option5);


            break;


        case 7:


            var option6 = document.createElement("OPTION");
            option6.value = "6";
            option6.text = "Pipes";
            mySelect.appendChild(option6);


            break;


        case 8:


            var option20 = document.createElement("OPTION");
            option20.value = "20";
            option20.text = "Faulty Traffic Light";
            mySelect.appendChild(option20);


            break;


        case 9:


            var option7 = document.createElement("OPTION");
            option7.value = "7";
            option7.text = "Pollution";
            mySelect.appendChild(option7);


            var option8 = document.createElement("OPTION");
            option8.value = "8";
            option8.text = "Leaking Pipe";
            mySelect.appendChild(option8);


            break;

    }
}
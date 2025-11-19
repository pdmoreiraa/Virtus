
// máscaras
$(document).ready(function () {
    $("#Telefone").mask("(00) 00000-0000");

    $("#CPF").mask("000.000.000-00");

    $("#CEP").mask("00000-000");

    $("#Validade").mask("00/00");
});

function Telefone(tel) {
    tel = tel.replace(/\D/g, "");

    if (tel.length === 11)
        return `(${tel.slice(0, 2)}) ${tel.slice(2, 7)}-${tel.slice(7)}`;

    if (tel.length === 10)
        return `(${tel.slice(0, 2)}) ${tel.slice(2, 6)}-${tel.slice(6)}`;

    return tel;
}

function CPF(cpf) {
    cpf = cpf.replace(/\D/g, "");
    if (cpf.length === 11) {
        return `${cpf.slice(0, 3)}.${cpf.slice(3, 6)}.${cpf.slice(6, 9)}-${cpf.slice(9)}`;
    }
    return cpf;
}

function CVV(cvv) {
    // Mantém só números
    cvv = cvv.replace(/\D/g, "");

    // Limita a 3 dígitos
    return cvv.slice(0, 3);
}


document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".telefone").forEach(el => {
        el.textContent = Telefone(el.textContent);
    });

    document.querySelectorAll(".cpf").forEach(el => {
        const original = el.textContent.trim();
        el.textContent = CPF(original);
    });

    const cvv = document.querySelector("#CVV");

    if (cvv) {
        cvv.addEventListener("input", function () {
            this.value = CVV(this.value);
        });
    }
});


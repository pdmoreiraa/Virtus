
// máscaras
$(document).ready(function () {
    $("#Telefone").mask("(00) 00000-0000");

    $("#CPF").mask("000.000.000-00");

    $("#CEP").mask("00000-000");
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

document.getElementById("Validade").addEventListener("input", function (e) {
    let v = e.target.value.replace(/\D/g, ""); // remove tudo que não é número

    if (v.length >= 3)
        e.target.value = v.slice(0, 2) + "/" + v.slice(2, 4);
    else
        e.target.value = v;
});

// VALIDAÇÃO DE MÊS E ANO
document.getElementById("Validade").addEventListener("blur", function () {
    const valor = this.value;

    if (!valor.includes("/")) {
        this.value = "";
        return;
    }

    const partes = valor.split("/");
    const mes = parseInt(partes[0]);
    const ano = parseInt(partes[1]);

    // validações
    if (
        isNaN(mes) || mes < 1 || mes > 12 ||   // mês inválido
        isNaN(ano) || ano < 0 || ano > 99      // ano inválido
    ) {
        alert("Data inválida. Use o formato MM/AA.");
        this.value = "";
    }
});

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


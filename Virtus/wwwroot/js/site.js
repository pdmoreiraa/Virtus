function salvarCarrinho(carrinho) {

    let carString = btoa(JSON.stringify(carrinho));

    let data = new Date();
    data.setDate(data.getDate() + 365);
    let expiracao = data.toUTCString();

    document.cookie = "shopping_cart=" + carString + ";expires=" + expiracao + ";path=/; SameSite=Strict; Secure";
}

// Função para adicionar um produto ao carrinho
function adicionarCarrinho(botao, id) {
    let carrinho = carrinhoDeCompras();

    let qtd = carrinho[id];
    if (isNaN(qtd)) {
        carrinho[id] = 1;
    } else {
        carrinho[id] = Number(qtd) + 1;
    }

    salvarCarrinho(carrinho);

    let tamanhoCarrinho = 0;
    for (var item of Object.entries(carrinho)) {
        qtd = item[1];
        if (isNaN(qtd)) continue;
        tamanhoCarrinho += Number(qtd);
    }

    document.getElementById("CartSize").innerHTML = tamanhoCarrinho;

    let produtoImg = document.getElementById(`produto-${id}`);
    let cartIcon = document.getElementById("cartIcon");

    if (!produtoImg) return;

    if (!produtoImg.complete) {
        produtoImg.onload = () => animarProduto(produtoImg, cartIcon);
    } else {
        animarProduto(produtoImg, cartIcon);
    }
}

function animarProduto(produtoImg, cartIcon) {
    const imgClone = produtoImg.cloneNode(true);
    document.body.appendChild(imgClone);

    // pega posições relativas à página
    const produtoRect = produtoImg.getBoundingClientRect();
    const cartRect = cartIcon.getBoundingClientRect();

    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    const scrollLeft = window.pageXOffset || document.documentElement.scrollLeft;

    const startX = produtoRect.left + scrollLeft;
    const startY = produtoRect.top + scrollTop;
    const endX = cartRect.left + scrollLeft;
    const endY = cartRect.top + scrollTop;

    imgClone.style.position = "absolute";
    imgClone.style.left = startX + "px";
    imgClone.style.top = startY + "px";
    imgClone.style.width = produtoRect.width + "px";
    imgClone.style.height = produtoRect.height + "px";
    imgClone.style.transition = "all 0.8s cubic-bezier(0.4, 0, 0.2, 1)";
    imgClone.style.zIndex = 9999;
    imgClone.style.pointerEvents = "none";
    imgClone.style.borderRadius = "10%";

    // força layout
    requestAnimationFrame(() => {
        imgClone.style.left = endX + "px";
        imgClone.style.top = endY + "px";
        imgClone.style.width = "0px";
        imgClone.style.height = "0px";
        imgClone.style.opacity = 0.5;
    });

    setTimeout(() => {
        imgClone.remove();
    }, 800);
}

// Função para aumentar a qtd de um item no carrinho
function aumentar(id) {
    let carrinho = carrinhoDeCompras();

    let qtd = carrinho[id];
    if (isNaN(qtd)) {
        carrinho[id] = 1;
    } else {
        carrinho[id] = Number(qtd) + 1;
    }

    salvarCarrinho(carrinho);
    location.reload();
}

// Função para diminuir a qtd de um item no carrinho
function diminuir(id) {
    let carrinho = carrinhoDeCompras();

    let qtd = carrinho[id];
    if (isNaN(qtd)) {
        return;
    }

    qtd = Number(qtd);

    if (qtd > 1) {
        carrinho[id] = qtd - 1;
        salvarCarrinho(carrinho);
        location.reload();
    }
}

// Função para remover um item do carrinho
function remover(id) {
    let carrinho = carrinhoDeCompras();

    if (carrinho[id]) {
        delete carrinho[id];
        salvarCarrinho(carrinho);
        location.reload();
    }
}

// Função auxiliar para obter o carrinho a partir do cookie
function carrinhoDeCompras() {
    const nomeCookie = "shopping_cart";
    let cookies = document.cookie.split(';');

    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i].trim();
        if (cookie.startsWith(nomeCookie + "=")) {
            let valor = cookie.substring(cookie.indexOf("=") + 1);
            try {
                let carrinho = JSON.parse(atob(valor));
                return carrinho;
            } catch (erro) {
                break;
            }
        }
    }

    return {};
}
function atualizarCarrinho() {
    const elemento = document.getElementById("CartSize");
    if (!elemento) return;

    let carrinho = CarrinhoDeCompras();

    let tamanhoCarrinho = 0;
    for (var item of Object.entries(carrinho)) {
        let qtd = item[1];
        if (isNaN(qtd)) continue;
        tamanhoCarrinho += Number(qtd);
    }

    elemento.innerHTML = tamanhoCarrinho;
}

// Chamada automática quando a página termina de carregar
window.addEventListener("DOMContentLoaded", () => {
    atualizarCarrinho();
});
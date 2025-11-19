function salvarCarrinho(carrinho) {

    let carrinhoStr = btoa(JSON.stringify(carrinho));

    let data = new Date();
    data.setDate(data.getDate() + 365);
    let expiracao = data.toUTCString();

    document.cookie = "shopping_cart=" + carrinhoStr + ";expires=" + expiracao + ";path=/; SameSite=Strict; Secure";
}

// Função para adicionar um produto ao carrinho
function adicionarAoCarrinho(botao, id) {
    let carrinho = obterCarrinhoDeCompras();

    let quantidade = carrinho[id];
    if (isNaN(quantidade)) {
        carrinho[id] = 1;
    } else {
        carrinho[id] = Number(quantidade) + 1;
    }

    salvarCarrinho(carrinho);

    let tamanhoCarrinho = 0;
    for (var item of Object.entries(carrinho)) {
        quantidade = item[1];
        if (isNaN(quantidade)) continue;
        tamanhoCarrinho += Number(quantidade);
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


// Função para aumentar a quantidade de um item no carrinho
function aumentar(id) {
    let carrinho = obterCarrinhoDeCompras();

    let quantidade = carrinho[id];
    if (isNaN(quantidade)) {
        carrinho[id] = 1;
    } else {
        carrinho[id] = Number(quantidade) + 1;
    }

    salvarCarrinho(carrinho);
    location.reload();
}

// Função para diminuir a quantidade de um item no carrinho
function diminuir(id) {
    let carrinho = obterCarrinhoDeCompras();

    let quantidade = carrinho[id];
    if (isNaN(quantidade)) {
        return;
    }

    quantidade = Number(quantidade);

    if (quantidade > 1) {
        carrinho[id] = quantidade - 1;
        salvarCarrinho(carrinho);
        location.reload();
    }
}

// Função para remover um item do carrinho
function remover(id) {
    let carrinho = obterCarrinhoDeCompras();

    if (carrinho[id]) {
        delete carrinho[id];
        salvarCarrinho(carrinho);
        location.reload();
    }
}

// Função auxiliar para obter o carrinho a partir do cookie
function obterCarrinhoDeCompras() {
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
function atualizarContadorCarrinho() {
    const elemento = document.getElementById("CartSize");
    if (!elemento) return; // Evita erro se o elemento não existir ainda

    let carrinho = obterCarrinhoDeCompras();

    let tamanhoCarrinho = 0;
    for (var item of Object.entries(carrinho)) {
        let quantidade = item[1];
        if (isNaN(quantidade)) continue;
        tamanhoCarrinho += Number(quantidade);
    }

    elemento.innerHTML = tamanhoCarrinho;
}

// Chamada automática quando a página termina de carregar
window.addEventListener("DOMContentLoaded", () => {
    atualizarContadorCarrinho();
});
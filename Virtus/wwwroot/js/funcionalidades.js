function togglePassword(inputId, iconId) {
    const input = document.getElementById(inputId);
    const icon = document.getElementById(iconId);

    icon.addEventListener("mousedown", (e) => {
        e.preventDefault(); // impede o blur (perder foco)
    });

    icon.addEventListener("click", () => {
        const isPassword = input.type === "password";
        input.type = isPassword ? "text" : "password";
        icon.classList.toggle("bi-eye", !isPassword);
        icon.classList.toggle("bi-eye-slash", isPassword);
        input.focus(); // mantém o foco no campo
    });
}

togglePassword("senha", "toggleSenha");
togglePassword("novaSenha", "toggleNovaSenha");
togglePassword("confirmarSenha", "toggleConfirmarSenha");


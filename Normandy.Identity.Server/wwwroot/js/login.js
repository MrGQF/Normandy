
(function () {
    document.getElementById("login-form").onsubmit = function () {
        var password = document.getElementById("password").value;
        password = "rbpjLPi/kaV3Cx0sAc6+TxfB0ENuKKJgLXbBJpBmybwukPYA4wqF2SSF5ZyqUYx9Y5CQEgJQdFIwyCWE3fHjvxQZGlSyZvrsIIonUlgoCq5Ey9HTT4yXM39K65DwK9ynf60n58m6ClyOnhwIGGJHuB0U6kLyAdnYp343imhR9GDh0tCiAYAZ0JuteNGjPGVEqZQO6FCay3BrZTZL4aOjF4m8LQyR1WL3JySuGJdT3csanNi6Y+VBvOrtvhGOPtNmvwyhsMWBQd8Zu0o7NCZKA097WP4VUSTDtIBijAog+CmuQv9j9GfuEujbJ2E9/2IgFh9o5a/CEMOtgCEDBiO/fQ=="
        document.getElementById("password").value = password;
        return true;
    }
})();
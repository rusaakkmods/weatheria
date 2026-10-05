const countrySelect = document.querySelector("#country-select");
const countryStatus = document.querySelector("#country-status");
const citySelect = document.querySelector("#city-select");
const cityStatus = document.querySelector("#city-status");
let cityRequestVersion = 0;

async function loadCountries() {
    countryStatus.textContent = "Loading countries...";

    try {
        const response = await fetch("/api/countries", {
            headers: { Accept: "application/json" }
        });

        if (!response.ok) {
            throw new Error(`Country request failed with status ${response.status}.`);
        }

        const countries = await response.json();
        if (!Array.isArray(countries)) {
            throw new Error("Country response was not a list.");
        }

        const countryOptions = countries
            .filter(country =>
                country &&
                typeof country.name === "string" &&
                typeof country.code === "string")
            .map(country => new Option(country.name, country.code));

        if (countries.length > 0 && countryOptions.length === 0) {
            throw new Error("Country response did not contain valid entries.");
        }

        countrySelect.replaceChildren(
            new Option("Select a country", ""),
            ...countryOptions);

        if (countryOptions.length === 0) {
            countryStatus.textContent = "No countries are currently available.";
            return;
        }

        countrySelect.disabled = false;
        countryStatus.textContent = `${countryOptions.length} countries available.`;
    } catch {
        countrySelect.replaceChildren(new Option("Countries unavailable", ""));
        countryStatus.textContent = "Unable to load countries. Refresh the page to try again.";
    }
}

async function loadCities(countryCode) {
    const requestVersion = ++cityRequestVersion;
    citySelect.disabled = true;
    citySelect.replaceChildren(new Option("Loading cities...", ""));
    cityStatus.textContent = "Loading cities...";

    try {
        const response = await fetch(
            `/api/countries/${encodeURIComponent(countryCode)}/cities`,
            { headers: { Accept: "application/json" } });

        if (!response.ok) {
            throw new Error(`City request failed with status ${response.status}.`);
        }

        const cities = await response.json();
        if (!Array.isArray(cities)) {
            throw new Error("City response was not a list.");
        }

        const cityOptions = cities
            .filter(city => city && typeof city.name === "string")
            .map(city => new Option(city.name, city.name));

        if (cities.length > 0 && cityOptions.length === 0) {
            throw new Error("City response did not contain valid entries.");
        }

        if (requestVersion !== cityRequestVersion) {
            return;
        }

        citySelect.replaceChildren(
            new Option(cityOptions.length > 0 ? "Select a city" : "No cities available", ""),
            ...cityOptions);

        if (cityOptions.length === 0) {
            cityStatus.textContent = "No cities are currently available for this country.";
            return;
        }

        citySelect.disabled = false;
        cityStatus.textContent = `${cityOptions.length} cities available.`;
    } catch {
        if (requestVersion !== cityRequestVersion) {
            return;
        }

        citySelect.replaceChildren(new Option("Cities unavailable", ""));
        cityStatus.textContent = "Unable to load cities. Change countries to try again.";
    }
}

if (countrySelect && citySelect && cityStatus) {
    countrySelect.addEventListener("change", () => {
        cityRequestVersion += 1;
        citySelect.disabled = true;
        citySelect.replaceChildren(new Option("Select a country first", ""));
        cityStatus.textContent = "Select a country first.";

        if (countrySelect.value) {
            loadCities(countrySelect.value);
        }
    });
}

if (countrySelect && countryStatus) {
    loadCountries();
}
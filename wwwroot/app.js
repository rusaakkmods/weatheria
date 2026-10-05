const countrySelect = document.querySelector("#country-select");
const countryStatus = document.querySelector("#country-status");

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

if (countrySelect && countryStatus) {
    loadCountries();
}
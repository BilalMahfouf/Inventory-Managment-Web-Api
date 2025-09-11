
// this function work !!!
export async function login(request) {

    try {
        const response = await fetch("/api/auth/login", {
            method: "Post",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(request)
        });
        const data = await response.json();
        return data;
    }
    catch (e) {
        console.log(`error in login function : ${e.message}`);
    }

}
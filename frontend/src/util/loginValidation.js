
const minPasswordLength = 6;

const isValidPassword = (password) => {
    return password.length > minPasswordLength;
}

export { isValidPassword };

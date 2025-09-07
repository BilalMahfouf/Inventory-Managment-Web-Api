
import Button from "@components/Button";


export  function Login(){

    return (
        <div className="grid grid-rows-3 ">
            <label >Email</label>
            <input type="text" value="" />

             <label >Password</label>
            <input type="text" value="" />

            <Button text="Login" />

        </div>
    )
}
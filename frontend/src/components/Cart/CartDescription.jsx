import {InputLabel} from "@components/Inputs";
import { Eye,EyeOff, Mail,Lock } from "lucide-react";
import PasswordInputLabel from "@components/PasswordInputLabel";
import Button from "@components/Button";

export default function CartDescription(){


    return (
        <div>        
            <form className="grid grid-cols-1  justify-items-start gap-6 ">
                <InputLabel placeholder="Email" label="Email" type="email" leftIcon={Mail} />
                <PasswordInputLabel placeholder="Password" label="Password" leftIcon={Lock} rightIcon={Eye}
             rightIconReplacment={EyeOff} />
           

            <Button children="Login"  />
            </form>
            </div>
    )
}
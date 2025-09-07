



export default function Button(props){

    return (
        <div>
        <button type="button" class="text-white bg-[#3b5998] hover:bg-[#3b5998]/90 focus:ring-4 
        focus:outline-none focus:ring-[#3b5998]/50 font-medium rounded-lg text-sm px-5 py-2.5
         text-center dark:focus:ring-[#3b5998]/55 me-2 mb-2">
        {props.text}
        </button>
        </div>
    )

}